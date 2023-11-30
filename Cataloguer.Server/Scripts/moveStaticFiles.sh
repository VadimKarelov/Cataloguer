#!/bin/bash

source_path=../frontend-app/build/
dest_path=./wwwroot/

# Перемещаем все файлы сборки
cp -r $source_path* $dest_path