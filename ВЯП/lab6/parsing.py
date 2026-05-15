import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))

import requests
from bs4 import BeautifulSoup
import pandas as pd

url = "http://quotes.toscrape.com/"
soup = BeautifulSoup(requests.get(url).text, "html.parser")

quotes = []
for block in soup.find_all("div", class_="quote"):
    text = block.find("span", class_="text").text.strip()
    author = block.find("small", class_="author").text.strip()
    quotes.append({"Цитата": text, "Автор": author})

pd.DataFrame(quotes).to_csv("quotes.csv", index=False, encoding="utf-8")
print(f"Готово! Сохранено {len(quotes)} цитат в quotes.csv")