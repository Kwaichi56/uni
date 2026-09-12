"""Generate editable UML diagrams for lab 12 in native diagrams.net format.

Run: python lab12/generate_diagrams.py
"""

from pathlib import Path
import xml.etree.ElementTree as ET


OUT = Path(__file__).with_name("SportNutritionShop_UML.drawio")
DOC = ET.Element("mxfile", host="app.diagrams.net", type="device", version="24.7.17")


class Page:
    def __init__(self, name, page_id, width=1800, height=1200):
        diagram = ET.SubElement(DOC, "diagram", id=page_id, name=name)
        model = ET.SubElement(
            diagram, "mxGraphModel", dx="1600", dy="900", grid="1", gridSize="10",
            guides="1", tooltips="1", connect="1", arrows="1", fold="1",
            page="1", pageScale="1", pageWidth=str(width), pageHeight=str(height),
            math="0", shadow="0",
        )
        self.root = ET.SubElement(model, "root")
        ET.SubElement(self.root, "mxCell", id="0")
        ET.SubElement(self.root, "mxCell", id="1", parent="0")
        # IDs 0 and 1 are reserved for the mxGraph root and default layer.
        self.count = 1

    def _id(self):
        self.count += 1
        return f"{self.count}"

    def box(self, label, x, y, w, h, style="rounded=1;whiteSpace=wrap;html=0;fillColor=#ffffff;strokeColor=#475569;fontColor=#0f172a;fontSize=15;", parent="1"):
        cell = ET.SubElement(self.root, "mxCell", id=self._id(), value=label,
                             style=style, vertex="1", parent=parent)
        ET.SubElement(cell, "mxGeometry", x=str(x), y=str(y), width=str(w),
                      height=str(h), **{"as": "geometry"})
        return cell.attrib["id"]

    def edge(self, source, target, label="", style="endArrow=block;html=0;rounded=0;strokeColor=#475569;fontColor=#334155;fontSize=13;", points=None):
        cell = ET.SubElement(self.root, "mxCell", id=self._id(), value=label,
                             style=style, edge="1", parent="1", source=source, target=target)
        geo = ET.SubElement(cell, "mxGeometry", relative="1", **{"as": "geometry"})
        if points:
            arr = ET.SubElement(geo, "Array", **{"as": "points"})
            for x, y in points:
                ET.SubElement(arr, "mxPoint", x=str(x), y=str(y))
        return cell.attrib["id"]

    def line(self, x1, y1, x2, y2, dashed=False, color="#64748b"):
        cell = ET.SubElement(self.root, "mxCell", id=self._id(), value="",
                             style=f"endArrow=none;startArrow=none;dashed={int(dashed)};strokeColor={color};",
                             edge="1", parent="1")
        geo = ET.SubElement(cell, "mxGeometry", relative="1", **{"as": "geometry"})
        ET.SubElement(geo, "mxPoint", x=str(x1), y=str(y1), **{"as": "sourcePoint"})
        ET.SubElement(geo, "mxPoint", x=str(x2), y=str(y2), **{"as": "targetPoint"})

    def text(self, label, x, y, w, h=30, size=15, bold=False, align="left"):
        return self.box(label, x, y, w, h,
                        f"text;html=0;whiteSpace=wrap;align={align};verticalAlign=middle;strokeColor=none;fillColor=none;fontColor=#0f172a;fontSize={size};fontStyle={1 if bold else 0};")

    def title(self, title, subtitle):
        self.text(title, 40, 20, 1300, 46, 28, True)
        self.text(subtitle, 42, 65, 1500, 30, 14)


def use_case():
    p = Page("1. Варианты использования", "use-cases", 1600, 1080)
    p.title("Диаграмма вариантов использования", "Магазин спортивного питания · роли и доступные действия")
    p.box("SportNutritionShop", 310, 125, 1120, 870,
          "rounded=0;whiteSpace=wrap;html=0;align=left;verticalAlign=top;spacing=14;fillColor=#f8fafc;strokeColor=#94a3b8;fontStyle=1;fontSize=17;")
    client = p.box("Клиент", 90, 350, 110, 120, "shape=umlActor;whiteSpace=wrap;html=0;fillColor=none;strokeColor=#334155;fontSize=17;")
    admin = p.box("Администратор", 80, 685, 140, 120, "shape=umlActor;whiteSpace=wrap;html=0;fillColor=none;strokeColor=#334155;fontSize=17;")
    cases = {}
    for key, label, x, y in [
        ("login", "Войти в систему", 395, 185), ("view", "Просматривать каталог", 680, 185),
        ("filter", "Искать и фильтровать", 1050, 185), ("cart", "Управлять корзиной", 395, 340),
        ("savecart", "Сохранить корзину", 705, 340), ("order", "Оформить заказ", 1050, 340),
        ("stock", "Проверить остатки", 1050, 485), ("profile", "Редактировать профиль", 395, 575),
        ("adminproducts", "Управлять товарами", 705, 680), ("db", "Администрировать БД", 1050, 680),
        ("stats", "Просмотреть статистику", 1050, 830),
    ]:
        cases[key] = p.box(label, x, y, 240, 66,
                           "ellipse;whiteSpace=wrap;html=0;fillColor=#dbeafe;strokeColor=#2563eb;fontColor=#1e3a8a;fontSize=15;")
    assoc = "endArrow=none;startArrow=none;strokeColor=#475569;"
    for k in ("login", "view", "cart", "order", "profile"):
        p.edge(client, cases[k], style=assoc)
    for k in ("adminproducts", "db"):
        p.edge(admin, cases[k], style=assoc)
    # Administrator specializes the client role.
    p.edge(admin, client, style="endArrow=block;endFill=0;strokeColor=#475569;")
    include = "dashed=1;endArrow=open;strokeColor=#64748b;fontSize=13;"
    extend = "dashed=1;endArrow=open;strokeColor=#a855f7;fontSize=13;"
    p.edge(cases["order"], cases["stock"], "«include»", include)
    p.edge(cases["filter"], cases["view"], "«extend»", extend)
    p.edge(cases["savecart"], cases["cart"], "«extend»", extend)
    p.edge(cases["stats"], cases["db"], "«extend»", extend)
    p.text("«include» — обязательный шаг; «extend» — дополнительное действие.", 370, 1010, 1050, 25, 13)


def uml_class(p, name, attrs, methods, x, y, w, h, color="#eff6ff"):
    cell = p.box("", x, y, w, h, f"rounded=0;fillColor={color};strokeColor=#475569;")
    p.text(name, x + 8, y + 4, w - 16, 33, 16, True, "center")
    p.line(x, y + 42, x + w, y + 42)
    ay = y + 50
    for a in attrs:
        p.text(a, x + 12, ay, w - 24, 23, 12)
        ay += 23
    p.line(x, ay + 4, x + w, ay + 4)
    my = ay + 10
    for m in methods:
        p.text(m, x + 12, my, w - 24, 23, 12)
        my += 23
    return cell


def classes():
    p = Page("2. Классы", "classes", 1950, 1420)
    p.title("Диаграмма классов", "Выбранные классы реального проекта · + public, − private · краткая сигнатура")
    vm = uml_class(p, "MainViewModel : BaseViewModel", [
        "+ Products: ObservableCollection<Product>", "+ CartItems: ObservableCollection<CartItem>",
        "+ CartTotal: decimal", "+ PlaceOrderCommand: ICommand", "− _undoRedo: UndoRedoManager",
    ], ["− PlaceOrder(): void", "− AddToCart(product: Product): void", "+ ReloadProducts(): void"], 50, 140, 380, 290)
    data = uml_class(p, "«static» DataService", [], [
        "+ LoadProducts(): ObservableCollection<Product>", "+ CreateOrder(login, items): void",
        "+ SaveProducts(products): void"], 570, 160, 380, 195, "#dcfce7")
    db = uml_class(p, "«static» DatabaseService", [], [
        "+ CreateOrderAsync(login, items): Task", "+ GetProductsAsync(): Task<List<Product>>",
        "+ TryLogin(login, password, out role): bool"], 1090, 160, 400, 195, "#dcfce7")
    uow = uml_class(p, "«interface» IShopUnitOfWork", [
        "+ Products: IProductRepository", "+ Users: IRepository<UserEntity>",
        "+ Orders: IRepository<OrderEntity>"], [
        "+ BeginTransactionAsync(): Task<IShopTransaction>", "+ SaveChangesAsync(): Task<int>"],
        1530, 140, 380, 265, "#fef3c7")
    ef = uml_class(p, "EfShopUnitOfWork", ["− _context: ShopDbContext"],
                   ["+ BeginTransactionAsync(): Task<IShopTransaction>", "+ SaveChangesAsync(): Task<int>"],
                   1530, 480, 380, 200, "#fef3c7")
    prod = uml_class(p, "ProductEntity", [
        "+ Id: int", "+ CategoryId: int", "+ ShortName: string", "+ Price: double",
        "+ Quantity: int", "+ SoldCount: int"], [], 50, 630, 300, 210)
    category = uml_class(p, "CategoryEntity", ["+ Id: int", "+ Name: string", "+ Products: List<ProductEntity>"],
                         [], 50, 950, 300, 150)
    user = uml_class(p, "UserEntity", ["+ Id: int", "+ Login: string", "+ Role: string", "+ Orders: List<OrderEntity>"],
                     [], 500, 950, 300, 175)
    order = uml_class(p, "OrderEntity", ["+ Id: int", "+ UserId: int", "+ Total: double", "+ Status: string",
                                      "+ Items: List<OrderItemEntity>"], [], 920, 930, 320, 195)
    item = uml_class(p, "OrderItemEntity", ["+ Id: int", "+ OrderId: int", "+ ProductId: int?",
                                         "+ Quantity: int", "+ UnitPrice: double"], [], 1350, 930, 320, 195)
    cart = uml_class(p, "CartItem : BaseViewModel", ["+ Product: Product", "+ Quantity: int", "+ Total: decimal"],
                     [], 460, 630, 310, 160)
    viewproduct = uml_class(p, "Product : BaseViewModel", ["+ Id: int", "+ ShortName: string", "+ Quantity: int",
                                                  "+ FinalPrice: decimal"], [], 900, 630, 310, 180)
    dep = "dashed=1;endArrow=open;strokeColor=#64748b;fontSize=12;"
    assoc = "endArrow=none;startArrow=none;strokeColor=#475569;fontSize=12;"
    comp = "startArrow=diamondThin;startFill=1;endArrow=none;strokeColor=#475569;fontSize=12;"
    p.edge(vm, data, "использует", dep)
    p.edge(data, db, "делегирует", dep)
    p.edge(db, uow, "использует", dep)
    p.edge(ef, uow, "реализует", "dashed=1;endArrow=block;endFill=0;strokeColor=#475569;")
    p.edge(vm, cart, "0..*", assoc)
    p.edge(cart, viewproduct, "1", assoc)
    p.edge(category, prod, "1 ↔ 0..*", assoc)
    p.edge(user, order, "1 ↔ 0..*", assoc)
    p.edge(order, item, "1 ↔ 1..*", comp)
    p.edge(item, prod, "0..1 ↔ 0..*", assoc)
    p.text("Модель UI Product отделена от сущности хранения ProductEntity.", 50, 1280, 1100, 30, 14)


def sequence():
    p = Page("3. Последовательность заказа", "sequence", 1750, 1300)
    p.title("Диаграмма последовательности", "Вариант использования «Оформить заказ» · успешная ветка и ошибки")
    names = ["Клиент", "MainWindow", "MainViewModel", "DataService", "DatabaseService", "IShopUnitOfWork / SQLite"]
    xs = [85, 345, 610, 880, 1150, 1460]
    for x, name in zip(xs, names):
        p.box(name, x - 85, 145, 175, 54,
              "rounded=0;whiteSpace=wrap;html=0;fillColor=#dbeafe;strokeColor=#2563eb;fontSize=14;")
        p.line(x, 200, x, 1190, True)
    def msg(a, b, y, label, ret=False):
        x1, x2 = xs[a], xs[b]
        p.text(label, min(x1, x2) + 10, y - 28, abs(x2 - x1) - 20, 25, 12)
        cell = ET.SubElement(p.root, "mxCell", id=p._id(), value="",
                             style=f"endArrow={'open' if ret else 'block'};dashed={int(ret)};strokeColor=#475569;",
                             edge="1", parent="1")
        geo = ET.SubElement(cell, "mxGeometry", relative="1", **{"as": "geometry"})
        ET.SubElement(geo, "mxPoint", x=str(x1), y=str(y), **{"as": "sourcePoint"})
        ET.SubElement(geo, "mxPoint", x=str(x2), y=str(y), **{"as": "targetPoint"})
    msg(0, 1, 260, "Нажать «Оформить заказ»")
    msg(1, 2, 320, "PlaceOrderCommand.Execute()")
    msg(2, 3, 390, "CreateOrder(login, cartItems)")
    msg(3, 4, 460, "CreateOrderAsync(login, snapshot)")
    msg(4, 5, 530, "BeginTransactionAsync()")
    msg(5, 4, 585, "transaction", True)
    msg(4, 5, 650, "Найти пользователя; добавить OrderEntity")
    p.box("loop: для каждого CartItem", 1030, 705, 690, 185,
          "rounded=0;fillColor=none;strokeColor=#94a3b8;dashed=1;align=left;verticalAlign=top;spacing=8;fontSize=13;")
    msg(4, 5, 770, "Найти ProductEntity; проверить остаток")
    msg(4, 5, 840, "Уменьшить Quantity; добавить OrderItemEntity")
    msg(4, 5, 945, "SaveChangesAsync(); CommitAsync()")
    msg(5, 4, 1000, "успех", True)
    msg(4, 3, 1050, "завершено", True)
    msg(3, 2, 1100, "завершено", True)
    msg(2, 1, 1150, "Очистить корзину; показать успех", True)
    p.text("alt: при ошибке проверки/БД — исключение; транзакция не фиксируется; ViewModel показывает сообщение об ошибке.",
           60, 1220, 1580, 38, 13)


def activity():
    p = Page("4. Деятельность заказа", "activity", 1550, 1230)
    p.title("Диаграмма деятельности", "Оформление заказа: проверки, транзакция и обновление интерфейса")
    start = p.box("", 715, 125, 30, 30, "ellipse;fillColor=#1e293b;strokeColor=#1e293b;")
    click = p.box("Нажать «Оформить заказ»", 595, 190, 270, 55, "rounded=1;fillColor=#dbeafe;strokeColor=#2563eb;fontSize=15;")
    nonempty = p.box("Корзина не пуста?", 630, 295, 200, 100, "rhombus;fillColor=#fef3c7;strokeColor=#d97706;fontSize=15;")
    begin = p.box("Начать транзакцию", 595, 455, 270, 55, "rounded=1;fillColor=#dcfce7;strokeColor=#16a34a;fontSize=15;")
    lookup = p.box("Найти пользователя и товар", 595, 550, 270, 55, "rounded=1;fillColor=#dcfce7;strokeColor=#16a34a;fontSize=15;")
    valid = p.box("Товар есть и остатка хватает?", 625, 660, 210, 110, "rhombus;fillColor=#fef3c7;strokeColor=#d97706;fontSize=14;")
    nextitem = p.box("Уменьшить остаток; добавить позицию", 580, 815, 300, 60, "rounded=1;fillColor=#dcfce7;strokeColor=#16a34a;fontSize=14;")
    more = p.box("Есть ещё позиции?", 630, 915, 200, 95, "rhombus;fillColor=#fef3c7;strokeColor=#d97706;fontSize=15;")
    commit = p.box("Сохранить заказ и зафиксировать транзакцию", 595, 1060, 300, 60, "rounded=1;fillColor=#dcfce7;strokeColor=#16a34a;fontSize=14;")
    success = p.box("Очистить корзину; показать успех", 1030, 1060, 300, 60, "rounded=1;fillColor=#dbeafe;strokeColor=#2563eb;fontSize=14;")
    error = p.box("Показать ошибку; не фиксировать транзакцию", 1050, 675, 290, 75, "rounded=1;fillColor=#fee2e2;strokeColor=#dc2626;fontSize=14;")
    end = p.box("", 1390, 1080, 32, 32, "ellipse;fillColor=#1e293b;strokeColor=#1e293b;")
    plain = "endArrow=block;strokeColor=#475569;fontSize=13;"
    for a, b, label in [(start, click, ""), (click, nonempty, ""), (nonempty, begin, "да"),
                        (begin, lookup, ""), (lookup, valid, ""), (valid, nextitem, "да"),
                        (nextitem, more, ""), (more, commit, "нет"), (commit, success, ""),
                        (success, end, ""), (valid, error, "нет"), (nonempty, error, "нет"), (error, end, "")]:
        p.edge(a, b, label, plain)
    p.edge(more, lookup, "да", "endArrow=block;edgeStyle=orthogonalEdgeStyle;strokeColor=#475569;fontSize=13;",
           [(440, 970), (440, 575)])


def components():
    p = Page("5. Компоненты и размещение", "components", 1700, 1040)
    p.title("Диаграмма компонентов и размещения", "Локальное WPF-приложение, EF Core и SQLite")
    p.box("«device» Компьютер пользователя", 65, 130, 1510, 800,
          "rounded=0;fillColor=#f8fafc;strokeColor=#475569;align=left;verticalAlign=top;spacing=14;fontStyle=1;fontSize=18;")
    p.box("«executionEnvironment» .NET 10 / WPF", 105, 210, 900, 630,
          "rounded=0;fillColor=#eff6ff;strokeColor=#2563eb;align=left;verticalAlign=top;spacing=14;fontStyle=1;fontSize=17;")
    view = p.box("«component» Views\nMainWindow · LoginWindow\nProfileWindow · DatabaseWindow", 150, 300, 300, 115,
                 "shape=component;whiteSpace=wrap;fillColor=#dbeafe;strokeColor=#2563eb;fontSize=15;")
    vm = p.box("«component» ViewModels\nMain · Login · Profile · DatabaseAdmin", 555, 300, 340, 115,
               "shape=component;whiteSpace=wrap;fillColor=#dbeafe;strokeColor=#2563eb;fontSize=15;")
    services = p.box("«component» Services\nDataService · DatabaseService\nAuthService · ProfileService", 550, 515, 345, 130,
                     "shape=component;whiteSpace=wrap;fillColor=#dcfce7;strokeColor=#16a34a;fontSize=15;")
    data = p.box("«component» Data\nEfShopUnitOfWork · Repositories\nShopDbContext · Entity models", 150, 615, 320, 135,
                 "shape=component;whiteSpace=wrap;fillColor=#fef3c7;strokeColor=#d97706;fontSize=15;")
    orm = p.box("«library» EF Core SQLite provider", 1070, 300, 380, 80,
                "rounded=1;fillColor=#f5f3ff;strokeColor=#7c3aed;fontSize=16;")
    sqlite = p.box("«artifact» SportNutritionShop.db\nSQLite · локальный файл", 1070, 580, 380, 105,
                   "shape=database;whiteSpace=wrap;fillColor=#f5f3ff;strokeColor=#7c3aed;fontSize=16;")
    dep = "dashed=1;endArrow=open;strokeColor=#475569;fontSize=13;"
    for a, b, label in [(view, vm, "Binding / ICommand"), (vm, services, "вызовы"),
                        (services, data, "IShopUnitOfWork"), (data, orm, "ORM"), (orm, sqlite, "SQL")]:
        p.edge(a, b, label, dep)
    p.text("Все компоненты работают на одном компьютере; отдельный сервер приложения не требуется.",
           110, 875, 1300, 30, 14)


for maker in (use_case, classes, sequence, activity, components):
    maker()

for diagram in DOC.findall("diagram"):
    cells = diagram.findall(".//mxCell")
    ids = [cell.get("id") for cell in cells]
    if len(ids) != len(set(ids)):
        raise ValueError(f"Duplicate cell ID on page {diagram.get('name')}")
    known = set(ids)
    for cell in cells:
        for attr in ("source", "target"):
            if cell.get(attr) and cell.get(attr) not in known:
                raise ValueError(f"Unknown {attr} on page {diagram.get('name')}")

ET.indent(DOC, space="  ")
OUT.write_bytes(ET.tostring(DOC, encoding="utf-8", xml_declaration=True))
print(f"Created {OUT} ({len(DOC)} pages)")
