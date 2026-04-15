fi = input('Введите свое имя и фамилию ')
fi_list = fi.split()

#list comprehension, преобразует итерируемый лист еще один 
fi_list_capp = [i.capitalize() for i in fi_list]
print(fi_list_capp)

fi_srt = '. '.join(i[0] for i in fi_list_capp) 
print(fi_srt+'.')


numbers = list(range(10,20))
numbers = [numbers[i]**2 for i in range(len(numbers))] 
print(numbers)
print(f"Сумма элементов: {sum(numbers)}")

filter_number = [i for i in numbers if i%2]
        
print(filter_number)
print(f"осталось элементов: {len(filter_number)}")


task3A = list(range(0, 20))

task3B = [sum(task3A[:i+1]) for i in range(len(task3A))]

print(task3B)


