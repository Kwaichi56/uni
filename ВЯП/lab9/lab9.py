import spacy
from wordcloud import WordCloud
import matplotlib.pyplot as plt
import numpy as np
from PIL import Image
import re
import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))

# ==========================================
# ЧАСТЬ 1: Семантический анализ текста
# ==========================================
print("--- ЧАСТЬ 1: Семантический анализ текста ---")

# 1. Текст на русском языке (4-6 предложений с сущностями)
text = (
    "Вчера Илон Маск, генеральный директор компании Tesla, посетил Москву. "
    "На встрече обсуждались инвестиции в размере 5 миллиардов рублей в новые технологии. "
    "Завод будет построен на берегу реки Волга, что ускорит логистику. "
    "Масса нового оборудования составляет около 500 килограммов. "
    "Александр Иванов, главный инженер проекта, отметил высокую важность этой сделки."
)


nlp = spacy.load("ru_core_news_sm")

# 3. Создание объекта Doc
doc = nlp(text)
print(f"Количество токенов в тексте: {len(doc)}")
print(f"Количество предложений: {len(list(doc.sents))}\n")

# 4. Разбиение на токены, вывод первых токенов и фильтрация
print("Первые 10 токенов и их свойства (Токен | Лемма | Часть речи | Стоп-слово):")
for token in list(doc)[:10]:
    print(f"{token.text:12} | {token.lemma_:10} | {token.pos_:6} | {token.is_stop}")

# Фильтрация стоп-слов, пунктуации и чисел
filtered_tokens = [
    token for token in doc 
    if not token.is_stop and not token.is_punct and not token.like_num
]

# 5. Вывод списка всех лемм после фильтрации
lemmas = [token.lemma_ for token in filtered_tokens]
print("\nСписок лемм после фильтрации:")
print(lemmas)

# 6. Определение именованных сущностей (NER)
print("\nИменованные сущности:")
for ent in doc.ents:
    print(f"Сущность: {ent.text:15} | Метка: {ent.label_}")

# ==========================================
# ЧАСТЬ 2: Облако слов с загрузкой из файла
# ==========================================
print("\n--- ЧАСТЬ 2: Облако слов ---")

def create_wordcloud(file_path, mask_path=None):
    # Проверка существования файла
    if not os.path.exists(file_path):
        print(f"Файл {file_path} не найден. Создайте его и добавьте туда текст.")
        return

    # 1. Подгрузка текста из файла
    with open(file_path, 'r', encoding='utf-8') as f:
        custom_text = f.read()
        
    if not custom_text.strip():
        print("Файл пуст. Добавьте текст для облака слов.")
        return

    # 2. Обработка текста с помощью SpaCy
    custom_doc = nlp(custom_text)
    
    # Фильтруем знаки препинания, стоп-слова, пробелы и приводим к леммам
    processed_words = [
        token.lemma_ for token in custom_doc 
        if not token.is_stop and not token.is_punct and not token.is_space
    ]
    processed_text = " ".join(processed_words)

    # 3. Придание облаку контуров фигуры (если передана маска)
    # Если маски нет, будет построен стандартный прямоугольник
    custom_mask = None
    if mask_path and os.path.exists(mask_path):
        custom_mask = np.array(Image.open(mask_path))

    # Генерация облака слов
    wordcloud = WordCloud(
        width=800, 
        height=800,
        background_color='white',
        mask=custom_mask,
        contour_width=1,
        contour_color='steelblue',
        min_font_size=10
    ).generate(processed_text)

    # Отрисовка
    plt.figure(figsize=(8, 8), facecolor=None)
    plt.imshow(wordcloud, interpolation="bilinear")
    plt.axis("off")
    plt.tight_layout(pad=0)
    plt.show()

# Запуск создания облака слов
# Для работы функции создайте текстовый файл, например 'my_text.txt', в папке со скриптом
text_file = input("Введите имя файла с текстом (например, data.txt): ")
create_wordcloud(text_file)