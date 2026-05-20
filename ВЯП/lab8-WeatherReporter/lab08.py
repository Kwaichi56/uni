import asyncio
import math
import os
import sqlite3
from datetime import datetime
from io import BytesIO
from zoneinfo import ZoneInfo

import aiohttp
from PIL import Image, ImageDraw
from aiogram import Bot, Dispatcher, F
from aiogram.exceptions import TelegramBadRequest
from aiogram.filters import Command, CommandStart
from aiogram.types import FSInputFile, Message, CallbackQuery
from aiogram.utils.keyboard import InlineKeyboardBuilder
from staticmap import StaticMap, CircleMarker


BOT_TOKEN = ""

bot = Bot(BOT_TOKEN)
dp = Dispatcher()

user_last_geo = {}


CITY_TO_RADAR_BY = {
    "минск": ("minsk", "Минская область"),
    "брест": ("brest", "Брестская область"),
    "витебск": ("vitebsk", "Витебская область"),
    "гродно": ("grodno", "Гродненская область"),
    "гомель": ("gomel", "Гомельская область"),
    "могилев": ("mogilev", "Могилевская область"),
    "могилёв": ("mogilev", "Могилевская область"),
}

OBLAST_KEYWORDS_BY = {
    "минская область": ("minsk", "Минская область"),
    "minsk region": ("minsk", "Минская область"),
    "брестская область": ("brest", "Брестская область"),
    "brest region": ("brest", "Брестская область"),
    "витебская область": ("vitebsk", "Витебская область"),
    "vitebsk region": ("vitebsk", "Витебская область"),
    "гродненская область": ("grodno", "Гродненская область"),
    "гродзенская вобласць": ("grodno", "Гродненская область"),
    "grodno region": ("grodno", "Гродненская область"),
    "гомельская область": ("gomel", "Гомельская область"),
    "gomel region": ("gomel", "Гомельская область"),
    "могилевская область": ("mogilev", "Могилевская область"),
    "могилёвская область": ("mogilev", "Могилевская область"),
    "mogilev region": ("mogilev", "Могилевская область"),
}


def normalize(text: str) -> str:
    return " ".join(text.strip().lower().split())


def init_db():
    conn = sqlite3.connect("weather_history.db")
    cur = conn.cursor()
    cur.execute("""
        CREATE TABLE IF NOT EXISTS requests (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            user_id INTEGER,
            username TEXT,
            query_text TEXT,
            city TEXT,
            region TEXT,
            country TEXT,
            country_code TEXT,
            latitude REAL,
            longitude REAL,
            created_at TEXT
        )
    """)
    conn.commit()
    conn.close()


def save_request_to_db(user_id: int, username: str | None, query_text: str, geo: dict):
    conn = sqlite3.connect("weather_history.db")
    cur = conn.cursor()
    cur.execute("""
        INSERT INTO requests (
            user_id, username, query_text, city, region, country, country_code,
            latitude, longitude, created_at
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (
        user_id,
        username,
        query_text,
        geo["city"],
        geo["region"],
        geo["country"],
        geo["country_code"],
        geo["lat"],
        geo["lon"],
        datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    ))
    conn.commit()
    conn.close()


async def geocode_city(city_query: str):
    url = (
        "https://geocoding-api.open-meteo.com/v1/search"
        f"?name={city_query}"
        "&count=10"
        "&language=ru"
        "&format=json"
    )

    async with aiohttp.ClientSession() as session:
        async with session.get(url, timeout=20) as resp:
            resp.raise_for_status()
            data = await resp.json()

    results = data.get("results", [])
    if not results:
        return None

    chosen = results[0]

    city = chosen.get("name", city_query)
    region = chosen.get("admin1") or chosen.get("admin2") or chosen.get("admin3") or "Неизвестный регион"
    country = chosen.get("country", "Неизвестная страна")
    country_code = chosen.get("country_code", "")
    timezone = chosen.get("timezone", "UTC")

    radar_slug_by = None
    oblast_by = None

    if country_code == "BY":
        city_norm = normalize(city)
        if city_norm in CITY_TO_RADAR_BY:
            radar_slug_by, oblast_by = CITY_TO_RADAR_BY[city_norm]
        else:
            joined_admin = " | ".join([
                str(chosen.get("admin1", "")),
                str(chosen.get("admin2", "")),
                str(chosen.get("admin3", "")),
                str(chosen.get("admin4", "")),
            ]).lower()

            for key, value in OBLAST_KEYWORDS_BY.items():
                if key in joined_admin:
                    radar_slug_by, oblast_by = value
                    break

        if oblast_by:
            region = oblast_by

    return {
        "city": city,
        "region": region,
        "country": country,
        "country_code": country_code,
        "lat": chosen["latitude"],
        "lon": chosen["longitude"],
        "timezone": timezone,
        "radar_slug_by": radar_slug_by,
    }


async def fetch_weather(lat: float, lon: float, timezone: str = "auto", days: int = 7):
    url = (
        "https://api.open-meteo.com/v1/forecast"
        f"?latitude={lat}&longitude={lon}"
        "&current=temperature_2m,relative_humidity_2m"
        "&hourly=temperature_2m,precipitation_probability"
        "&daily=temperature_2m_min,temperature_2m_max,precipitation_probability_max,weather_code"
        f"&timezone={timezone}"
        f"&forecast_days={days}"
    )

    async with aiohttp.ClientSession() as session:
        async with session.get(url, timeout=20) as resp:
            resp.raise_for_status()
            return await resp.json()


def build_3hour_forecast(hourly: dict) -> str:
    times = hourly["time"]
    temps = hourly["temperature_2m"]
    probs = hourly.get("precipitation_probability", [])

    lines = []
    for i in range(0, min(len(times), 24), 3):
        time_part = times[i].split("T")[1]
        prob = probs[i] if i < len(probs) else "?"
        lines.append(f"{time_part} — {temps[i]}°C, осадки: {prob}%")

    return "\n".join(lines)


def get_local_datetime_text(geo: dict) -> str:
    timezone_name = geo.get("timezone", "UTC")
    try:
        now_local = datetime.now(ZoneInfo(timezone_name))
    except Exception:
        now_local = datetime.now()

    return now_local.strftime("%d.%m.%Y %H:%M:%S")


def get_forecast_keyboard():
    builder = InlineKeyboardBuilder()
    builder.button(text="Сегодня", callback_data="forecast:today")
    builder.button(text="Завтра", callback_data="forecast:tomorrow")
    builder.button(text="3 дня", callback_data="forecast:3days")
    builder.button(text="7 дней", callback_data="forecast:7days")
    builder.adjust(2, 2)
    return builder.as_markup()


async def download_radar_veg_by(radar_slug: str) -> str | None:
    if not radar_slug:
        return None

    update_url = f"https://radar.veg.by/data/{radar_slug}/update.json"

    async with aiohttp.ClientSession() as session:
        async with session.get(update_url, timeout=20) as resp:
            resp.raise_for_status()
            update_data = await resp.json()

        times = update_data.get("times", [])
        if not times:
            return None

        last_time = times[-1]
        image_url = f"https://radar.veg.by/data/{radar_slug}/images/{last_time}.png"

        async with session.get(image_url, timeout=30) as img_resp:
            img_resp.raise_for_status()
            content_type = img_resp.headers.get("Content-Type", "")
            if not content_type.startswith("image/"):
                return None
            image_bytes = await img_resp.read()

    os.makedirs("radar_images", exist_ok=True)
    file_path = f"radar_images/vegby_{radar_slug}_{last_time}.png"

    with open(file_path, "wb") as f:
        f.write(image_bytes)

    return file_path


def deg2num(lat_deg, lon_deg, zoom):
    lat_rad = math.radians(lat_deg)
    n = 2.0 ** zoom
    xtile = int((lon_deg + 180.0) / 360.0 * n)
    ytile = int((1.0 - math.asinh(math.tan(lat_rad)) / math.pi) / 2.0 * n)
    return xtile, ytile


async def download_rainviewer_tile(session, host, path, z, x, y):
    url = f"{host}{path}/256/{z}/{x}/{y}/2/1_1.png"
    async with session.get(url, timeout=30) as resp:
        resp.raise_for_status()
        data = await resp.read()
        return Image.open(BytesIO(data)).convert("RGBA")


async def build_world_radar_map(geo: dict) -> str | None:
    lat = geo["lat"]
    lon = geo["lon"]
    zoom = 7
    width = 768
    height = 768

    async with aiohttp.ClientSession(headers={
        "User-Agent": "WeatherTelegramBot/1.0 (student educational project)"
    }) as session:
        async with session.get("https://api.rainviewer.com/public/weather-maps.json", timeout=20) as resp:
            resp.raise_for_status()
            rv = await resp.json()

        host = rv.get("host")
        past = rv.get("radar", {}).get("past", [])
        if not host or not past:
            return None

        frame = past[-1]
        path = frame.get("path")
        frame_time = frame.get("time")
        if not path:
            return None

        m = StaticMap(width, height, url_template="https://tile.openstreetmap.org/{z}/{x}/{y}.png")
        marker = CircleMarker((lon, lat), "#e53935", 14)
        m.add_marker(marker)
        base_img = m.render(zoom=zoom).convert("RGBA")

        center_x, center_y = deg2num(lat, lon, zoom)
        canvas = Image.new("RGBA", (width, height), (0, 0, 0, 0))
        start_x = center_x - 1
        start_y = center_y - 1

        for ix in range(3):
            for iy in range(3):
                tile_x = start_x + ix
                tile_y = start_y + iy
                try:
                    tile = await download_rainviewer_tile(session, host, path, zoom, tile_x, tile_y)
                    tile = tile.resize((256, 256))
                    canvas.paste(tile, (ix * 256, iy * 256), tile)
                except Exception:
                    pass

        overlay = canvas.crop((0, 0, width, height))
        result = Image.alpha_composite(base_img, overlay)

        draw = ImageDraw.Draw(result)
        draw.rectangle((10, 10, 380, 60), fill=(255, 255, 255, 215))
        draw.text((20, 22), f"{geo['city']}, {geo['country']}", fill=(20, 20, 20))

    os.makedirs("radar_images", exist_ok=True)
    safe_city = geo["city"].replace(" ", "_").replace("/", "_")
    file_path = f"radar_images/world_{safe_city}_{frame_time}.png"
    result.save(file_path)

    return file_path


async def try_get_radar_image(geo: dict):
    if geo["country_code"] == "BY" and geo["radar_slug_by"]:
        try:
            file_path = await download_radar_veg_by(geo["radar_slug_by"])
            if file_path:
                return "VEG.BY", file_path
        except Exception:
            pass

    try:
        file_path = await build_world_radar_map(geo)
        if file_path:
            return "RainViewer + OpenStreetMap", file_path
    except Exception:
        pass

    return None, None


def format_today_message(geo: dict, weather_data: dict, radar_source: str | None) -> str:
    current = weather_data["current"]
    daily = weather_data["daily"]
    hourly = weather_data["hourly"]

    lat = round(geo["lat"], 4)
    lon = round(geo["lon"], 4)
    radar_text = radar_source if radar_source else "недоступна"
    local_dt = get_local_datetime_text(geo)

    return (
        f"Дата и время: {local_dt}\n\n"
        f"Город: {geo['city']}\n"
        f"Регион: {geo['region']}\n"
        f"Страна: {geo['country']}\n"
        f"Географические координаты: {lat}, {lon}\n\n"
        f"Текущая температура: {current['temperature_2m']}°C\n"
        f"Влажность: {current['relative_humidity_2m']}%\n"
        f"Минимальная температура за день: {daily['temperature_2m_min'][0]}°C\n"
        f"Максимальная температура за день: {daily['temperature_2m_max'][0]}°C\n"
        f"Вероятность осадков: {daily['precipitation_probability_max'][0]}%\n\n"
        f"Температура каждые 3 часа:\n{build_3hour_forecast(hourly)}\n\n"
        f"Источник карты: {radar_text}"
    )


def format_daily_block(daily: dict, start_index: int, days_count: int) -> str:
    times = daily["time"]
    t_min = daily["temperature_2m_min"]
    t_max = daily["temperature_2m_max"]
    precip = daily["precipitation_probability_max"]

    lines = []
    for i in range(start_index, min(start_index + days_count, len(times))):
        lines.append(
            f"{times[i]}:\n"
            f"Мин: {t_min[i]}°C\n"
            f"Макс: {t_max[i]}°C\n"
            f"Вероятность осадков: {precip[i]}%\n"
        )
    return "\n".join(lines)


def format_forecast_message(geo: dict, weather_data: dict, mode: str) -> str:
    local_dt = get_local_datetime_text(geo)
    daily = weather_data["daily"]

    header = (
        f"Дата и время: {local_dt}\n\n"
        f"Город: {geo['city']}\n"
        f"Регион: {geo['region']}\n"
        f"Страна: {geo['country']}\n"
        f"Координаты: {round(geo['lat'], 4)}, {round(geo['lon'], 4)}\n\n"
    )

    if mode == "today":
        return header + "Прогноз на сегодня:\n\n" + format_daily_block(daily, 0, 1)
    if mode == "tomorrow":
        return header + "Прогноз на завтра:\n\n" + format_daily_block(daily, 1, 1)
    if mode == "3days":
        return header + "Прогноз на 3 дня:\n\n" + format_daily_block(daily, 0, 3)
    if mode == "7days":
        return header + "Прогноз на 7 дней:\n\n" + format_daily_block(daily, 0, 7)

    return header + "Нет данных."


@dp.message(CommandStart())
async def cmd_start(message: Message):
    await message.answer(
        "Привет! Я бот погоды.\n\n"
        "Отправь название города.\n"
        "После ответа появятся кнопки:\n"
        "- Сегодня\n"
        "- Завтра\n"
        "- 3 дня\n"
        "- 7 дней\n\n"
        "Команды: /start, /help"
    )


@dp.message(Command("help"))
async def cmd_help(message: Message):
    await message.answer(
        "Просто отправь название города, например:\n"
        "- Минск\n"
        "- Берлин\n"
        "- Токио\n\n"
        "После ответа нажимай кнопки под сообщением для переключения прогноза."
    )


@dp.message(F.photo)
async def on_photo(message: Message):
    await message.answer("Я получил фото. Для прогноза отправь название города текстом.")


@dp.message(F.document)
async def on_document(message: Message):
    await message.answer("Я получил файл. Для прогноза отправь название города текстом.")


@dp.message(F.sticker)
async def on_sticker(message: Message):
    await message.answer("Стикер получил. Но для прогноза нужен город текстом.")


@dp.message(F.text)
async def on_text(message: Message):
    city_query = message.text.strip()

    geo = await geocode_city(city_query)
    if not geo:
        await message.answer("Не удалось найти такой город. Попробуй написать точнее.")
        return

    user_last_geo[message.from_user.id] = geo

    try:
        weather_data = await fetch_weather(geo["lat"], geo["lon"], geo["timezone"], days=7)
    except Exception:
        await message.answer("Не удалось получить данные о погоде.")
        return

    try:
        save_request_to_db(
            user_id=message.from_user.id,
            username=message.from_user.username,
            query_text=city_query,
            geo=geo
        )
    except Exception:
        pass

    radar_source, radar_path = await try_get_radar_image(geo)
    text = format_today_message(geo, weather_data, radar_source)
    keyboard = get_forecast_keyboard()

    if radar_path and os.path.exists(radar_path):
        try:
            await message.answer_photo(
                photo=FSInputFile(radar_path),
                caption=text,
                reply_markup=keyboard
            )
        except TelegramBadRequest:
            await message.answer_document(
                document=FSInputFile(radar_path),
                caption=text,
                reply_markup=keyboard
            )
    else:
        await message.answer(text, reply_markup=keyboard)


@dp.callback_query(F.data.startswith("forecast:"))
async def forecast_callback(callback: CallbackQuery):
    user_id = callback.from_user.id
    geo = user_last_geo.get(user_id)

    if not geo:
        await callback.answer("Сначала отправь город.", show_alert=True)
        return

    mode = callback.data.split(":")[1]

    try:
        weather_data = await fetch_weather(geo["lat"], geo["lon"], geo["timezone"], days=7)
    except Exception:
        await callback.answer("Не удалось обновить прогноз.", show_alert=True)
        return

    text = format_forecast_message(geo, weather_data, mode)

    try:
        await callback.message.edit_caption(
            caption=text,
            reply_markup=get_forecast_keyboard()
        )
    except Exception:
        try:
            await callback.message.edit_text(
                text=text,
                reply_markup=get_forecast_keyboard()
            )
        except Exception:
            pass

    await callback.answer()


@dp.message()
async def fallback(message: Message):
    await message.answer("Я не понимаю. Отправь название города или используй /help.")


async def main():
    init_db()
    await dp.start_polling(bot)


if __name__ == "__main__":
    asyncio.run(main())
