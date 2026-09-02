import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))

import pandas as pd
import matplotlib.pyplot as plt

df = pd.read_csv("quotes.csv")

print("1) Первые 5 строк:\n", df.head(), "\n")

df["Длина"] = df["Цитата"].str.len()

print("2) Первые 10 строк с новыми столбцами:\n", df.head(10), "\n")
print("3) Статистика:\n", df["Длина"].describe(), "\n")
print("4) Средняя длина цитат по авторам:\n", df.groupby("Автор")["Длина"].mean(), "\n")

fig, axes = plt.subplots(1, 2, figsize=(10, 4))
df["Длина"].plot.hist(ax=axes[0], title="Гистограмма длины", color="blue", edgecolor="black")
df.boxplot(column="Длина", ax=axes[1])
axes[1].set_title("Box-plot длины")

plt.savefig("quotes_charts.png")
plt.close()

veg = pd.read_csv("Vegetabls_sales.csv", sep=";")

prod = veg.groupby("Product")["Sales"].sum()
prod_city = veg.groupby(["Product", "City"])["Sales"].sum()

print("\nТоп-3 самых продаваемых продукта:\n", prod.sort_values(ascending=False).head(3))

fig, axes = plt.subplots(1, 2, figsize=(12, 5))
prod.plot.bar(ax=axes[0], title="Продажи по продуктам", color="purple")
prod_city.unstack().plot.bar(ax=axes[1], title="По продуктам и городам", stacked=True)
plt.savefig("vegetables_charts.png")
plt.close()
print("Графики сохранены в vegetables_charts.png")