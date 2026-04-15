# Задание 1
def uppercase(func):
    def wrapper(*args, **kwargs):
        result = func(*args, **kwargs)
        return result.upper()
    return wrapper

@uppercase
def name(string):
    return f"hello, {string}"

print(name("oleg"))
print(name("python"))


# Задание 2
def count_calls(func):
    def wrapper(*args, **kwargs):
        wrapper.call_count += 1
        return func(*args, **kwargs)
    wrapper.call_count = 0
    return wrapper

@count_calls
def greet(name):
    print(f"Hello, {name}!")

greet("tom")
greet("denis")
print(f"function greet has been called {greet.call_count} times")


# Задание 3
def html_tag(tag):
    def decorator(func):
        def wrapper(*args, **kwargs):
            result = func(*args, *kwargs)
            return f"<{tag}>{result}</{tag}>"
        return wrapper
    return decorator

@html_tag("div")
def get_text():
    return "Hellow, World!"
print(get_text())
