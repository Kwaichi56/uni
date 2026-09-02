print('Задание 1')

a = int(input("Введите целочисленное число а: "))
b = float(input("Введите вещественное число b: "))


print(f"сумма чисел = {a+b}\n")

print(f"произведение чисел = {a*b}\n")
print("деление чисел = " + str(round(a/b,2)))
print(f"разность чисел = {a-b}")

if a %2:
    print('нечетное')
else:
    print('четное')

# print('нечетное') if a%2 else print('четное')


print('Задание 2')

s = input('Введите строку: ')
print(s)
print("длина строки = " + str(len(s)))
print("строка в верхнем регистре: " + s.upper())

# print(s.isdigit()) 

for i in s:
    if i.isdigit():
        print('строка содержит число')
        break

print(s[len(s)//2:])
print(s[:len(s)//2])


print('Задание 3')

numbers = [1,2,3,4,5,3,6,7,8,3,9,54,66,3,4,67]
print(f"текущий список: {numbers}")

print(f"последний элемент: {numbers[-1]}")

numbers.append(34)
print(f"добавлен элемент: {numbers}")

print(f"число 3 встречается {numbers.count(3)} раза")

numbers.sort()
print(numbers)

usStr = input("Введите строку: ")
user = usStr.split()
print(user)
print("количество слов в стоке" + len(user))



print('Задание 4')
numbers = list(range(11,21))
for i in range(len(numbers)):
    numbers.append(numbers[i]**2)

print(numbers)
