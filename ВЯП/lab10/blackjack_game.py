import random
import sys
from dataclasses import dataclass

import pygame

from blackjack_view import (
    SCREEN_WIDTH,
    SCREEN_HEIGHT,
    FPS,
    CARD_W,
    CARD_GAP,
    DEALER_Y,
    PLAYER_Y,
    DECK_POS,
    Button,
    draw_scene,
)

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
        total = sum(card.value for card in self.cards)
        aces = sum(1 for card in self.cards if card.rank == 'A')
        while total > 21 and aces:
            total -= 10
            aces -= 1
        return total

    def is_blackjack(self):
        return len(self.cards) == 2 and self.value() == 21

    def is_bust(self):
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


class AnimatedCard:
    def __init__(self, owner, index, card, start, end, face_up=True, speed=1250):
        self.owner = owner
        self.index = index
        self.card = card
        self.start = pygame.Vector2(start)
        self.end = pygame.Vector2(end)
        self.pos = pygame.Vector2(start)
        self.face_up = face_up
        self.speed = speed
        self.done = False

    def update(self, dt):
        if self.done:
            return
        delta = self.end - self.pos
        dist = delta.length()
        if dist <= self.speed * dt or dist == 0:
            self.pos = self.end.copy()
            self.done = True
            return
        self.pos += delta * min(1.0, self.speed * dt / dist)


class BlackjackGame:
    def __init__(self):
        pygame.init()
        pygame.display.set_caption('Blackjack')
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        self.clock = pygame.time.Clock()
        self.running = True

        self.shoe = Shoe(4)
        self.player = Hand()
        self.dealer = Hand()
        self.animations = []

        self.balance = 1000
        self.bet = 50
        self.message = 'Нажмите "Раздача".'
        self.reveal_dealer = False
        self.state = 'menu'
        self.phase = 'betting'
        self.round_end_wait = 0.0

        bw, bh = 240, 68
        self.menu_play = Button(SCREEN_WIDTH // 2 - 120, 360, bw, bh, 'Играть')
        self.menu_exit = Button(SCREEN_WIDTH // 2 - 120, 446, bw, bh, 'Выйти')

        self.btn_bet_up = Button(90, 758, 190, 56, 'Повысить ставку')
        self.btn_bet_down = Button(300, 758, 190, 56, 'Понизить ставку')
        self.btn_deal = Button(540, 758, 170, 56, 'Раздача')
        self.btn_hit = Button(760, 758, 190, 56, 'Добрать карту')
        self.btn_stand = Button(970, 758, 170, 56, 'Не брать')
        self.btn_replay = Button(1160, 758, 210, 56, 'Играть заново')

    def reset_hands(self):
        self.player = Hand()
        self.dealer = Hand()
        self.animations = []
        self.reveal_dealer = False
        self.round_end_wait = 0.0

    def enter_betting_phase(self):
        self.reset_hands()
        self.phase = 'betting'
        self.message = 'Нажмите "Раздача".'
        if self.balance >= MIN_BET:
            self.bet = max(MIN_BET, min(self.bet, self.balance))
        else:
            self.bet = MIN_BET

    def can_interact(self):
        return self.phase == 'player_turn' and not self.animations

    def hand_positions(self, owner, count):
        start_x = SCREEN_WIDTH // 2 - ((count - 1) * (CARD_W + CARD_GAP)) // 2 - CARD_W // 2
        y = PLAYER_Y if owner == 'player' else DEALER_Y
        return [pygame.Vector2(start_x + i * (CARD_W + CARD_GAP), y) for i in range(count)]

    def deal_card(self, owner, face_up=True):
        hand = self.player if owner == 'player' else self.dealer
        card = self.shoe.draw()
        hand.add(card)
        index = len(hand.cards) - 1
        pos = self.hand_positions(owner, len(hand.cards))[index]
        self.animations.append(AnimatedCard(owner, index, card, DECK_POS, pos, face_up=face_up))

    def start_round(self):
        if self.balance < self.bet:
            self.message = 'Недостаточно фишек.'
            return
        self.reset_hands()
        self.balance -= self.bet
        self.phase = 'dealing'
        self.message = 'Раздача...'
        self.deal_card('player', True)
        self.deal_card('dealer', False)
        self.deal_card('player', True)
        self.deal_card('dealer', True)

    def settle_round(self, outcome):
        self.phase = 'round_over'
        self.reveal_dealer = True
        if outcome == 'player_blackjack':
            payout = int(self.bet * 2.5)
            self.balance += payout
            self.message = 'Блекджек!'
        elif outcome == 'dealer_blackjack':
            self.message = 'У дилера блекджек'
        elif outcome == 'player_bust':
            self.message = 'Перебор'
        elif outcome == 'dealer_bust':
            self.balance += self.bet * 2
            self.message = 'Дилер перебрал'
        elif outcome == 'player_win':
            self.balance += self.bet * 2
            self.message = 'Победа'
        elif outcome == 'push':
            self.balance += self.bet
            self.message = 'Ничья'
        else:
            self.message = 'Дилер победил'
        self.round_end_wait = 0.35

    def check_initial_blackjack(self):
        if len(self.player.cards) < 2 or len(self.dealer.cards) < 2 or self.animations:
            return
        if self.player.is_blackjack() and self.dealer.is_blackjack():
            self.settle_round('push')
        elif self.player.is_blackjack():
            self.settle_round('player_blackjack')
        elif self.dealer.is_blackjack():
            self.settle_round('dealer_blackjack')
        else:
            self.phase = 'player_turn'
            self.message = 'Ваш ход'

    def player_hit(self):
        if not self.can_interact():
            return
        self.deal_card('player', True)
        self.message = 'Добираем карту...'

    def player_stand(self):
        if not self.can_interact():
            return
        self.reveal_dealer = True
        self.phase = 'dealer_turn'
        self.message = 'Ход дилера'

    def dealer_turn(self):
        if self.animations:
            return
        if self.dealer.value() < 17:
            self.deal_card('dealer', True)
            return
        player_value = self.player.value()
        dealer_value = self.dealer.value()
        if self.dealer.is_bust():
            self.settle_round('dealer_bust')
        elif dealer_value > player_value:
            self.settle_round('dealer_win')
        elif dealer_value < player_value:
            self.settle_round('player_win')
        else:
            self.settle_round('push')

    def is_animating_card(self, owner, index):
        return any(anim.owner == owner and anim.index == index for anim in self.animations)

    def update_round(self, dt):
        for anim in self.animations:
            anim.update(dt)
        self.animations = [anim for anim in self.animations if not anim.done]

        if self.state != 'table':
            return

        if self.phase == 'dealing' and not self.animations:
            self.check_initial_blackjack()

        if self.phase == 'player_turn' and not self.animations:
            if self.player.is_bust():
                self.settle_round('player_bust')
            else:
                self.message = 'Ваш ход'

        if self.phase == 'dealer_turn' and not self.animations:
            if self.round_end_wait > 0:
                self.round_end_wait = max(0, self.round_end_wait - dt)
            else:
                self.dealer_turn()

    def visible_dealer_cards(self):
        items = []
        for i, card in enumerate(self.dealer.cards):
            face_up = self.reveal_dealer or i > 0
            items.append((card, face_up))
        return items

    def static_layout(self):
        layout = []
        player_positions = self.hand_positions('player', len(self.player.cards))
        for i, card in enumerate(self.player.cards):
            if not self.is_animating_card('player', i):
                layout.append((card, player_positions[i], True))
        dealer_positions = self.hand_positions('dealer', len(self.dealer.cards))
        for i, (card, face_up) in enumerate(self.visible_dealer_cards()):
            if not self.is_animating_card('dealer', i):
                layout.append((card, dealer_positions[i], face_up))
        return layout

    def animated_layout(self):
        return [(anim.card, anim.pos, anim.face_up) for anim in self.animations]

    def change_bet(self, delta):
        if self.phase != 'betting' or self.animations:
            return
        if self.balance < MIN_BET:
            self.message = 'Недостаточно фишек.'
            return
        self.bet = max(MIN_BET, min(self.balance, self.bet + delta))
        self.message = 'Нажмите "Раздача".'

    def mouse_down(self, pos):
        if self.state == 'menu':
            if self.menu_play.contains(pos):
                self.state = 'table'
                self.enter_betting_phase()
            elif self.menu_exit.contains(pos):
                self.running = False
            return

        if self.btn_bet_up.contains(pos):
            self.change_bet(BET_STEP)
        elif self.btn_bet_down.contains(pos):
            self.change_bet(-BET_STEP)
        elif self.btn_deal.contains(pos) and self.phase == 'betting' and not self.animations:
            self.start_round()
        elif self.btn_hit.contains(pos):
            self.player_hit()
        elif self.btn_stand.contains(pos):
            self.player_stand()
        elif self.btn_replay.contains(pos) and self.phase == 'round_over' and not self.animations:
            self.enter_betting_phase()

    def update_hover(self, pos):
        buttons = [
            self.menu_play, self.menu_exit,
            self.btn_bet_up, self.btn_bet_down, self.btn_deal,
            self.btn_hit, self.btn_stand, self.btn_replay,
        ]
        for button in buttons:
            button.hovered = button.contains(pos) and button.visible and button.enabled

    def sync_buttons(self):
        betting = self.phase == 'betting' and not self.animations
        player_turn = self.phase == 'player_turn' and not self.animations
        replay = self.phase == 'round_over' and not self.animations

        self.btn_bet_up.visible = True
        self.btn_bet_down.visible = True
        self.btn_deal.visible = True
        self.btn_hit.visible = True
        self.btn_stand.visible = True
        self.btn_replay.visible = replay

        self.btn_bet_up.enabled = betting and self.balance >= MIN_BET and self.bet < self.balance
        self.btn_bet_down.enabled = betting and self.balance >= MIN_BET and self.bet > MIN_BET
        self.btn_deal.enabled = betting and self.balance >= self.bet and self.balance >= MIN_BET
        self.btn_hit.enabled = player_turn
        self.btn_stand.enabled = player_turn
        self.btn_replay.enabled = replay

    def game_loop(self):
        while self.running:
            dt = self.clock.tick(FPS) / 1000
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    self.running = False
                elif event.type == pygame.MOUSEMOTION:
                    self.update_hover(event.pos)
                elif event.type == pygame.MOUSEBUTTONDOWN and event.button == 1:
                    self.mouse_down(event.pos)
                elif event.type == pygame.KEYDOWN and self.state == 'table':
                    if event.key == pygame.K_h:
                        self.player_hit()
                    elif event.key == pygame.K_s:
                        self.player_stand()
                    elif event.key == pygame.K_RETURN and self.phase == 'betting':
                        self.start_round()
                    elif event.key == pygame.K_ESCAPE:
                        self.state = 'menu'
                        self.enter_betting_phase()

            self.update_round(dt)
            self.sync_buttons()
            draw_scene(
                self.screen,
                self.state,
                self.menu_play,
                self.menu_exit,
                self.btn_hit,
                self.btn_stand,
                self.btn_bet_up,
                self.btn_bet_down,
                self.btn_deal,
                self.btn_replay,
                self.balance,
                self.bet,
                self.message,
                len(self.shoe.cards),
                self.static_layout(),
                self.animated_layout(),
                self.player.value() if self.player.cards else None,
                self.dealer.value() if self.reveal_dealer else None,
                self.phase,
            )
            pygame.display.flip()

        pygame.quit()
        sys.exit()


if __name__ == '__main__':
    BlackjackGame().game_loop()