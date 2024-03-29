# Аналитическая система.  Формирование эффективного каталога товаров.

Формулировка задания:

Предоставление пользователю средств построения каталога для рассылки потенциальным клиентам в следующем периоде.
Пользователь посредством экранных форм должен сформировать каталог, указать тираж, направление рассылки, количество экземпляров отправляемых по каждому направлению и сохранить всю информацию в БД системы. Далее посредством запуска процедуры расчета Система должна построить прогноз продаж по сформированному каталогу.

### Схемы

![Image](Labs/ЛР2.png)

### Правила

1. Для таблицы возрастной группы в полях минимального и максимального возраста будем указывать границы включительно.

2. Каталог составляется для нескольких мест рассылки, `ТИРАЖ` в таблице каталога должен быть суммой всех рассылок.


### Общая структура проекта

Фронтенд - TypeScript + React

Бэкенд - C# + EF Core + ASP.NET + PostgreSQL

### Требования по разработке

1. PostgreSQL 16

2. Ветви желательно именовать C-"номер issue на github", например `C-85`. Можно добавить краткое описание, например `C-85-add-base`. Если нет issue, то допускается использовать любое наименование ветвей.

### Структура программы

1. Пространства имен для C# классов должны соответствовать папкам, в которых они расположены.

2. Для базы данных будет создан отдельный проект.

3. Для обмена данными между бэкендом и фронтендом будет использоваться JSON формат. Для удобства в папке `Models` необходимо создать модели, описывающие передваваемую информацию.

### Запуск ASP.NET

##### Через консоль

- Prod версия: Запустить скрипт соотв. ОС:
  - Windows: .\Scripts\runReleaseBuild.bat
  - Unix: ./Scripts/runReleaseBuild.sh
- Dev (без сборки frontend): Запустить скрипт соотв. ОС:
  - Windows: .\Scripts\runDebugBuild.bat
  - Unix: ./Scripts/runDebugBuild.sh

##### Через Visual Studio

1. Выбрать в качестве запускаемого проекта Cataloguer.Server

2. Запустить через F5 или Ctrl+F5

### Запросы из фронтенда к ASP.NET

##### Получение коллекций без параметров

Путь: `/api/v1/cataloguer/getBrochures` - получение списка всех каталогов

Путь: `/api/v1/cataloguer/getAgeGroups` - получение списка всех возрастных групп
