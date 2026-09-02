import spacy
from wordcloud import WordCloud
import matplotlib.pyplot as plt
import numpy as np
from PIL import Image
import re
import os

os.chdir(os.path.dirname(os.path.abspath(__file__)))

print("Семантический анализ текста")

text = (
    "Вчера Илон Маск, генеральный директ;р компании Tesla, посетил Москву. "
    "На встрече обсуждались инвестиции в размере 5 миллиардов рублей в новые технологии. "
    "Завод будет построен на берегу реки Волга, что ускорит логистику. "
    "Масса нового оборудования составляет около 500 килограммов. "
    "Александр Иванов, главный инженер проекта, отметил высокую важность этой сделки."
)

nlp = spacy.load("ru_core_news_sm")

doc = nlp(text)
print(f"Количество токенов в тексте: {len(doc)}")
print(f"Количество предложений: {len(list(doc.sents))}\n")

print("Первые 10 токенов и их свойства (Токен | Лемма | Часть речи | Стоп-слово):")
for token in list(doc)[:10]:
    print(f"{token.text:12} | {token.lemma_:10} | {token.pos_:6} | {token.is_stop}")

filtered_tokens = [
    token for token in doc
    if not token.is_stop and not token.is_punct and not token.like_num
]

lemmas = [token.lemma_ for token in filtered_tokens]
print("\nСписок лемм после фильтрации:")
print(lemmas)

print("\nИменованные сущности:")
for ent in doc.ents:
    print(f"Сущность: {ent.text:15} | Метка: {ent.label_}")

print("\n--- ЧАСТЬ 2: Облако слов ---")


def create_wordcloud(file_path, mask_path=None):
    if not os.path.exists(file_path):
        print(f"Файл {file_path} не найден. Создайте его и добавьте туда текст.")
        return

    with open(file_path, 'r', encoding='utf-8') as f:
        custom_text = f.read()

    if not custom_text.strip():
        print("Файл пуст. Добавьте текст для облака слов.")
        return

    custom_doc = nlp(custom_text)

    processed_words = [
        token.lemma_ for token in custom_doc
        if not token.is_stop and not token.is_punct and not token.is_space
    ]
    processed_text = " ".join(processed_words)

    custom_mask = None
    width = 1600
    height = 800
    font_path = None

    if mask_path and os.path.exists(mask_path):
        mask_image = Image.open(mask_path).convert("L")
        custom_mask = np.array(mask_image)
        height, width = custom_mask.shape

    wordcloud = WordCloud(
        width=width,
        height=height,
        background_color='white',
        mask=custom_mask,
        contour_width=2,
        contour_color='black',
        collocations=False,
        font_path=font_path,
        max_words=300,
        min_font_size=10
    ).generate(processed_text)

    plt.figure(figsize=(8, 8), facecolor=None)
    plt.imshow(wordcloud, interpolation="bilinear")
    plt.axis("off")
    plt.tight_layout(pad=0)
    plt.show()


text_file = input("Введите имя файла с текстом (например, data.txt): ")
mask_file = input("Введите имя файла маски (например, mask_news.png): ").strip()

if mask_file:
    create_wordcloud(text_file, mask_file)
else:
    create_wordcloud(text_file)