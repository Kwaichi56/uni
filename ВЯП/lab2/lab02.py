import random;
import math;

# Задание 1
task1 = tuple(random.randint(0,1000) for _ in range(10))

print('sum: ' + str(sum(task1)))
print('avg: ' + str(sum(task1)/len(task1)))
print('max: ' + str(max(task1)))
print('min: ' + str(min(task1)))
print(task1) 

# Задание 2
task2_list = list(task1)
print(task2_list)

print(f"Размер листа: {task2_list.__sizeof__()}")
print(f"Размер кортежа: {task1.__sizeof__()}")

# Задание 3

tuple_1 = (11,12,13,[10,20,30])

n = tuple_1[3]
i = n[0]
b = n[-1]
print(f"Первый элемент списка: {i}")
print(f"Последний элемент списка: {b}")

# Задание 4

first, nenelast, nelast, last = tuple_1

print(first)
print(nenelast)
print(nelast)
print(last)

# Задание 5

employers = {'Vovan': 45333,
             'Kiral': 123123,
             'chimichanga': 3123123}

print(employers)

n = employers.get("Kiral")
print(n)
n = employers.get("lalka")
print(n)
employers["chihachbill"] = 66788922
print(employers)

print(f"Длина словаря: {len(employers)}")

print(f"Количество записей в словаре: {employers.keys()}")

average = sum(employers.values())/len(employers)
print(f"Cреднее значение запрат: {average}")


# Задание 6
order_1 = {'apple', 'orange', 'banana'}
order_2 = {'apple', 'pera', 'orange'}


print(f"Объединение: {order_1 | order_2}") 
print(f"Пересечение: {order_1 & order_2}")
print(f"Разность(1 - 2): {order_1 - order_2}")
print(f"Не общие: {order_1 ^ order_2}")

order_1.update(order_2)
print(order_1)

# Задание 7
def square(x):
    return [x*4, x*x, x*math.sqrt(2)]

sp=square(5)
a, b, c = sp
print(a)
print(square(5))
 

def is_year_leap(year):
    if (year %4 == 0 and year %100 != 0) or (year %400 == 0):
        print('высокосный')
    else:
        print('не высокосный')

is_year_leap(2000)
is_year_leap(2100)
is_year_leap(1700)
is_year_leap(4)
    