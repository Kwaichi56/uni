import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))
import spacy
from wordcloud import WordCloud
import matplotlib.pyplot as plt
import numpy as np
from PIL import Image

nlp = spacy.load('ru_core_news_sm')

with open('search.txt', 'r', encoding='utf-8') as file:
    text = file.read()

doc = nlp(text)

print('Количество токенов в тексте:', len(doc))
print('\nПредложения:')
for sent in doc.sents:
    print('-', sent.text)

print('\nПервые 30 токенов:')

for token in doc[:30]:
    print(token.text, '| лемма:', token.lemma_, '| часть речи:', token.pos_, '| стоп-слово:', token.is_stop)

filtered_tokens = [token for token in doc if not token.is_stop and not token.is_punct and not token.like_num and not token.is_space]

print('\nОтфильтрованные токены:')

for token in filtered_tokens:
    print(token.text)

lemmas = [token.lemma_.lower() for token in filtered_tokens]
print('\nСписок лемм:')
print(lemmas)

print('\nВсе найденные именованные сущности:')

for ent in doc.ents:
    print(ent.text, '|', ent.label_)

print('\nГеографические названия, имена людей и организации:')

for ent in doc.ents:
    if ent.label_ in ['PER', 'ORG', 'LOC']:
        print(ent.text, '|', ent.label_)

custom_stop_words = {'это', 'который', 'также', 'сегодня', 'заявить', 'сообщить', 'отметить', 'рассказать', 'год', 'беларусь', 'рб'}
processed_words = []
for token in doc:
    lemma = token.lemma_.lower()
    if not token.is_stop and not token.is_punct and not token.like_num and not token.is_space and lemma not in custom_stop_words and len(lemma) > 2:
        processed_words.append(lemma)

processed_text = ' '.join(processed_words)

mask_image = Image.open("mask_news.png").convert("L")
mask = np.array(mask_image)

height, width = mask.shape
font_path = None

if processed_text.strip():
    wordcloud = WordCloud(
        width=width,
        height=height,
        background_color='white',
        mask=mask,
        contour_width=2,
        contour_color='black',
        collocations=False,
        font_path=font_path,
        max_words=300
    ).generate(processed_text)
    plt.figure(figsize=(16, 9))
    plt.imshow(wordcloud, interpolation='bilinear')
    plt.axis('off')
    plt.tight_layout(pad=0)
    plt.savefig('wordcloud_news.png', bbox_inches='tight', pad_inches=0.1)
    plt.show()
    print('\nОблако слов сохранено в файл wordcloud_news.png')
else:
    print('\nНедостаточно слов для построения облака слов')