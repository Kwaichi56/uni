class Character:
    def __init__(self, name, hp):
        self.name = name
        self.hp = hp

    def attack(self, target):
        print("Я атакую")


class Warrior(Character):
    def __init__(self, name, hp, atk, weapon):
        super().__init__(name, hp)
        self.set_atk(atk)
        self.set_weapon(weapon)

    def set_atk(self, atk):
        if atk < 0:
            self.atk = 0
        else:
            self.atk = atk

    def get_atk(self):
        return self.atk

    def set_weapon(self, weapon):
        if weapon == "":
            self.weapon = "Без оружия"
        else:
            self.weapon = weapon

    def get_weapon(self):
        return self.weapon

    def attack(self, target):
        super().attack(target)
        print("Я атакую мечом")
        self.atk -= 5
        target.hp -= 10


class Mage(Character):
    def __init__(self, name, hp, atk, weapon):
        super().__init__(name, hp)
        self.set_atk(atk)
        self.set_weapon(weapon)

    def set_atk(self, atk):
        if atk < 0:
            self.atk = 0
        else:
            self.atk = atk

    def get_atk(self):
        return self.atk

    def set_weapon(self, weapon):
        if weapon == "":
            self.weapon = "Без оружия"
        else:
            self.weapon = weapon

    def get_weapon(self):
        return self.weapon

    def attack(self, target):
        super().attack(target)
        print("Я атакую магией")
        self.atk -= 8
        target.hp -= 15


class Archer(Character):
    def __init__(self, name, hp, atk, weapon):
        super().__init__(name, hp)
        self.set_atk(atk)
        self.set_weapon(weapon)

    def set_atk(self, atk):
        if atk < 0:
            self.atk = 0
        else:
            self.atk = atk

    def get_atk(self):
        return self.atk

    def set_weapon(self, weapon):
        if weapon == "":
            self.weapon = "Без оружия"
        else:
            self.weapon = weapon

    def get_weapon(self):
        return self.weapon

    def attack(self, target):
        super().attack(target)
        print("Я атакую стрелами")
        self.atk -= 6
        target.hp -= 12

warrior = Warrior("Федос", 150, 20, "Меч")
mage = Mage("Иван", 100, 25, "Посох")
archer = Archer("Максим", 70, 40, "Лук")

heroes = [warrior, mage, archer]


print("Информация о персонажах")
for hero in heroes:
    print(f"Имя: {hero.name}, Жизни: {hero.hp}, Силы: {hero.get_atk()}, Оружие: {hero.get_weapon()}")

print("Бой")
warrior.attack(mage)
mage.attack(archer)
archer.attack(warrior)

power = [warrior.get_atk(), mage.get_atk(), archer.get_atk()]

max_atk = max(power)
winner_index = power.index(max_atk)
winner = heroes[winner_index]

# max_atk = -1
# winner_name = ""

# for hero in heroes:
#     if hero.get_atk() > max_atk:
#         max_atk = hero.get_atk()
#         winner_name = hero.name

print("Итог")
print(f"Больше всего сил осталось у: {winner.name} (Остаток сил: {max_atk})")