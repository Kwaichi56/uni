import requests
from bs4 import BeautifulSoup
import csv

# Шаг 1: Получаем HTML-страницу
url = 'http://books.toscrape.com/'
response = requests.get(url)
soup = BeautifulSoup(response.text, 'lxml')

# Шаг 2: Ищем все карточки с товарами
books = soup.find_all('article', class_='product_pod')

data = []
for book in books:
    title = book.h3.a['title']
    price_str = book.find('p', class_='price_color').text
    # Очищаем цену от символа фунта и преобразуем в число
    price = float(price_str.replace('Â£', ''))
    data.append([f"{title}, \t{price_str}"])

# Шаг 3: Сохраняем в CSV
with open('books.csv', 'w', newline='', encoding='utf-8') as file:
    writer = csv.writer(file, delimiter=';')
    writer.writerow(['Title', '\t\t\t\t\tPrice'])
    writer.writerows(data)