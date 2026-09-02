import math

class Vector:
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __str__(self):
        return f"Vector({self.x}, {self.y})"

    def __eq__(self, other):
        if not isinstance(other, Vector):
            return False
        return self.x == other.x and self.y == other.y
    
    def __add__(self, other):
        return Vector(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vector(self.x - other.x, self.y - other.y)

    # Умножение на число 
    def __mul__(self, other):
        if not isinstance(other, (int, float)):
            return False
        return Vector(self.x * other, self.y * other)

    # Умножение числа на вектор 
    def __rmul__(self, other):
        if not isinstance(other, Vector):
            return False
        return self.__mul__(other)

    def __abs__(self):
        return math.sqrt(self.x**2 + self.y**2)

    def __neg__(self):
        return Vector(-self.x, -self.y)

    def __len__(self):
        return 2

    def __bool__(self):
        return bool(self.x or self.y)

v1 = Vector(3, 4)
v2 = Vector(1, 2)
v3 = Vector(3, 4)
v4 = Vector(0, 0)

print(v1)
print(v2 == v1)
print(v1 == v3)
print(f"Сложение: {v1 + v2}")        
print(f"Вычитание: {v1 - v2}")       
print(f"Умножение на 5: {v1 * 5}")   
print(f"5 умножить на v1: {5 * v1}") 
print(f"Длина вектора: {abs(v1)}")   
print(f"Отрицание: {-v1}")          
print(f"Размерность: {len(v1)}")    
print(f"Не нулевой? {bool(v1)}") 
print(f"Не нулевой? {bool(v4)}") 
