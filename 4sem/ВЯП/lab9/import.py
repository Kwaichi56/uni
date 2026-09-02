import json
import os
os.chdir(os.path.dirname(os.path.abspath(__file__)))


# Открываем исходный JSON-файл для чтения
with open('result.json', 'r', encoding='utf-8') as infile:
    data = json.load(infile)

# Открываем или создаем TXT-файл для записи результатов
with open('messages.txt', 'w', encoding='utf-8') as outfile:
    
    # Проходимся циклом по всем сообщениям в списке
    for message in data.get('messages', []):
        text_field = message.get('text', '')
        
        # Если текст внутри сообщения — это простая строка
        if isinstance(text_field, str):
            if text_field.strip():  # Проверяем, что строка не пустая
                outfile.write(text_field + '\n')
                
        # Если текст — это список из кусочков (так бывает при наличии ссылок или жирного шрифта)
        elif isinstance(text_field, list):
            full_text = ''
            for part in text_field:
                if isinstance(part, str):
                    full_text += part
                elif isinstance(part, dict):
                    full_text += part.get('text', '')
            
            if full_text.strip():  # Проверяем, что собранный текст не пустой
                outfile.write(full_text + '\n')