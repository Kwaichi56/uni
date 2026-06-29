CREATE TABLE IF NOT EXISTS Celebrities (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Firstname TEXT    NOT NULL,
    Surname   TEXT    NOT NULL,
    PhotoPath TEXT    NOT NULL
);

INSERT INTO Celebrities (Id, Firstname, Surname, PhotoPath) VALUES
(1, 'Noam',   'Chomsky',     '/Photo/Chomsky.jpg'),
(2, 'Tim',    'Berners-Lee', '/Photo/Berners-Lee.jpg'),
(3, 'Edgar',  'Codd',        '/Photo/Codd.jpg'),
(4, 'Donald', 'Knuth',       '/Photo/Knuth.jpg'),
(5, 'Linus',  'Torvalds',    '/Photo/Torvalds.jpg'),
(6, 'John',   'Neumann',     '/Photo/Neumann.jpg'),
(7, 'Edsgar', 'Dijkstra',    '/Photo/Dijkstra.jpg');
