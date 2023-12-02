@echo off

set "source_path=..\frontend-app\build\"
set "dest_path=.\wwwroot\"
set "static_route=static\*"
set "source_path_static=%source_path%%static_route%"
set "dest_path_static=%dest_path%%static_route%"

REM Перемещаем содержимое папки static
xcopy /e /v %source_path_static% %dest_path_static%

REM Перемещаем все файлы сборки
xcopy "%source_path%*" "%dest_path%*"