import random
import sys
from dataclasses import dataclass
import pygame
from blackjack_view import *

SUITS = ['♠', '♥', '♦', '♣']
RANKS = ['A', '2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K']
VALUES = {'A': 11, '2': 2, '3': 3, '4': 4, '5': 5, '6': 6, '7': 7, '8': 8, '9': 9, '10': 10, 'J': 10, 'Q': 10, 'K': 10}
MIN_BET = 10
BET_STEP = 10

@dataclass
class Card:
    suit: str
    rank: str
    @property
    def value(self):
        return VALUES[self.rank]

class Hand:
    def __init__(self):
        self.cards = []
    def add(self, card):
        self.cards.append(card)
    def value(self):
        total = sum(c.value for c in self.cards)
        aces = sum(c.rank == 'A' for c in self.cards)
        while total > 21 and aces:
            total -= 10
            aces -= 1
        return total
    def blackjack(self):
        return len(self.cards) == 2 and self.value() == 21
    def bust(self):
        return self.value() > 21

class Shoe:
    def __init__(self, decks=4):
        self.decks = decks
        self.cards = []
        self.reset()
    def reset(self):
        self.cards = [Card(s, r) for _ in range(self.decks) for s in SUITS for r in RANKS]
        random.shuffle(self.cards)
    def draw(self):
        if len(self.cards) < 40:
            self.reset()
        return self.cards.pop()

class Game:
    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((W, H))
        pygame.display.set_caption('Blackjack')
        self.clock = pygame.time.Clock()
        self.shoe = Shoe()
        self.balance = 1000
        self.bet = 50
        self.state = 'menu'
        self.status = 'Нажмите "Раздача".'
        self.reveal = False
        self.player = Hand()
        self.dealer = Hand()
        self.buttons = {
            'play': Btn(W // 2 - 120, 355, 240, 66, 'Играть'),
            'exit': Btn(W // 2 - 120, 438, 240, 66, 'Выйти'),
            'up': Btn(90, 758, 190, 56, 'Повысить ставку'),
            'down': Btn(300, 758, 190, 56, 'Понизить ставку'),
            'deal': Btn(540, 758, 170, 56, 'Раздача'),
            'hit': Btn(760, 758, 190, 56, 'Добрать карту'),
            'stand': Btn(970, 758, 170, 56, 'Не брать'),
            'again': Btn(1160, 758, 210, 56, 'Играть заново'),
        }

    def reset_round(self):
        self.player = Hand()
        self.dealer = Hand()
        self.reveal = False

    def betting(self):
        self.reset_round()
        self.state = 'betting'
        self.bet = max(MIN_BET, min(self.bet, max(self.balance, MIN_BET)))
        self.status = 'Нажмите "Раздача".'

    def deal_start(self):
        if self.balance < self.bet:
            self.status = 'Недостаточно фишек.'
            return
        self.reset_round()
        self.balance -= self.bet
        for owner in ('player', 'dealer', 'player', 'dealer'):
            card = self.shoe.draw()
            (self.player if owner == 'player' else self.dealer).add(card)
        self.state = 'playing'
        self.check_start()
        if self.state == 'playing':
            self.status = 'Ваш ход'

    def finish(self, result):
        self.state = 'round_over'
        self.reveal = True
        if result == 'bj':
            self.balance += int(self.bet * 2.5)
            self.status = 'Блекджек!'
        elif result == 'dealer_bj':
            self.status = 'У дилера блекджек'
        elif result == 'bust':
            self.status = 'Перебор'
        elif result == 'dealer_bust':
            self.balance += self.bet * 2
            self.status = 'Дилер перебрал'
        elif result == 'win':
            self.balance += self.bet * 2
            self.status = 'Победа'
        elif result == 'push':
            self.balance += self.bet
            self.status = 'Ничья'
        else:
            self.status = 'Дилер победил'

    def check_start(self):
        if self.player.blackjack() and self.dealer.blackjack():
            self.finish('push')
        elif self.player.blackjack():
            self.finish('bj')
        elif self.dealer.blackjack():
            self.finish('dealer_bj')

    def dealer_play(self):
        self.reveal = True
        while self.dealer.value() < 17:
            self.dealer.add(self.shoe.draw())
        dv, pv = self.dealer.value(), self.player.value()
        if dv > 21:
            self.finish('dealer_bust')
        elif dv > pv:
            self.finish('lose')
        elif dv < pv:
            self.finish('win')
        else:
            self.finish('push')

    def click(self, pos):
        b = self.buttons
        if self.state == 'menu':
            if b['play'].hit(pos): self.betting()
            elif b['exit'].hit(pos): return False
            return True
        if self.state == 'betting':
            if b['up'].hit(pos):
                self.bet = min(self.balance, self.bet + BET_STEP) if self.balance >= MIN_BET else self.bet
                self.bet = max(self.bet, MIN_BET)
                self.status = 'Нажмите "Раздача".'
            elif b['down'].hit(pos):
                self.bet = max(MIN_BET, self.bet - BET_STEP)
                self.status = 'Нажмите "Раздача".'
            elif b['deal'].hit(pos):
                self.deal_start()
        elif self.state == 'playing':
            if b['hit'].hit(pos):
                self.player.add(self.shoe.draw())
                if self.player.bust(): self.finish('bust')
                else: self.status = 'Ваш ход'
            elif b['stand'].hit(pos):
                self.status = 'Ход дилера'
                self.dealer_play()
        elif self.state == 'round_over' and b['again'].hit(pos):
            self.betting()
        return True

    # def key(self, key):
    #     if self.state == 'table':
    #         return
    #     if key == pygame.K_ESCAPE:
    #         self.state = 'menu'
    #     elif self.state == 'betting' and key == pygame.K_RETURN:
    #         self.deal_start()
    #     elif self.state == 'playing' and key == pygame.K_h:
    #         self.player.add(self.shoe.draw())
    #         if self.player.bust(): self.finish('bust')
    #     elif self.state == 'playing' and key == pygame.K_s:
    #         self.dealer_play()

    def visible_dealer(self):
        if self.reveal:
            return [(c, True) for c in self.dealer.cards]
        return [(c, i > 0) for i, c in enumerate(self.dealer.cards)]

    def layouts(self):
        dealer = [(c, pos_cards(len(self.dealer.cards), DEALER_Y)[i], face) for i, (c, face) in enumerate(self.visible_dealer())]
        player = [(c, pos_cards(len(self.player.cards), PLAYER_Y)[i], True) for i, c in enumerate(self.player.cards)]
        return dealer + player

    def run(self):
        while True:
            for e in pygame.event.get():
                if e.type == pygame.QUIT:
                    pygame.quit(); sys.exit()
                if e.type == pygame.MOUSEBUTTONDOWN and e.button == 1:
                    if not self.click(e.pos):
                        pygame.quit(); sys.exit()
                if e.type == pygame.KEYDOWN:
                    self.key(e.key)
            draw(self.screen, self.state, self.buttons, self.balance, self.bet, self.status, len(self.shoe.cards), self.layouts(), self.player.value() if self.player.cards else None, self.dealer.value() if self.reveal else None)
            pygame.display.flip()
            self.clock.tick(FPS)

if __name__ == '__main__':
    Game().run()
