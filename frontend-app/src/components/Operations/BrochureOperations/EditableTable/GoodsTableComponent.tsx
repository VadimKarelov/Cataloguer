import {Table} from "antd";
import React, {useEffect, useState} from "react";
import {BaseStoreInjector, GoodsExtendedProps, GoodsProps} from "../../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../../../styles/GoodsTableComponent.css";
import {EditableCellComponent} from "./EditableCellComponent";
import {EditableRowComponent} from "./EditableRowComponent";

/**
 * Коллекция свойств столбцов таблицы.
 */
const columns = [
    {
        title: "Название",
        dataIndex: "name",
        key: "goods_table_name",
        width: 170,
        editable: false,
    },
    {
        title: "Цена",
        dataIndex: "price",
        key: "goods_table_cost",
        width: 170,
        editable: true,
    },
];

/**
 * Свойства компонента GoodsTableComponent.
 */
interface GoodsTableComponentProps extends BaseStoreInjector {
}

/**
 * Компонент таблицы товаров при создании каталога.
 */
const GoodsTableComponent: React.FC<GoodsTableComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Товары.
     */
    const [goods, setGoods] = useState<GoodsExtendedProps[]>([]);

    /**
     * Инициализирует товары.
     * @param goods Товары.
     */
    const initGoods = (goods: GoodsProps[]) => {
        if (goods.length === 0) return;

        setGoods(
            goods.map(good => ({...good, key: `good_${good.id}`, isChecked: false}))
        );
    };

    /**
     * Хук, вызывающий инициализацию товаров.
     */
    useEffect(() => initGoods(props.brochureStore?.allGoods ?? []), [props.brochureStore?.allGoods]);

    /**
     * Свойства, необходимые для обработки выделяемых строк по checkbox.
     */
    const rowSelection = {
        onChange: (selectedRowKeys: React.Key[], selectedRows: GoodsExtendedProps[]) => {
            const selectedGoods = goods.map(currentGood => {
                currentGood.isChecked = selectedRowKeys.some(selectedRowKey => selectedRowKey === `good_${currentGood.id}`);
                return currentGood;
            });
            props.brochureStore?.setCheckedGoods(selectedGoods);
            setGoods(selectedGoods);
        },
        getCheckboxProps: (record: any) => ({
            key: record.key,
            id: record.id,
            name: record.name,
            price: record.price,
            isChecked: record.isChecked,
        }),
    };

    /**
     * Обрабатывает сохранение изменений в ячейке.
     * @param row Строка таблицы.
     */
    const handleSave = (row: GoodsExtendedProps) => {
        const newData = [...goods];
        const index = newData.findIndex((item) => row.key === item.key);
        const item = newData[index];
        newData.splice(index, 1, {
            ...item,
            ...row,
        });
        props.brochureStore?.setCheckedGoods(newData);
        setGoods(newData);
    };

    /**
     * Используемые компоненты для таблицы.
     * Используем редактируемую строку и редактирруемую ячейку.
     */
    const components = {
        body: {
            row: EditableRowComponent,
            cell: EditableCellComponent,
        },
    };

    /**
     * Колонки таблицы.
     */
    const cols = columns.map((col) => {
        if (!col.editable) {
            return col;
        }
        return {
            ...col,
            onCell: (record: GoodsExtendedProps) => ({
                record,
                editable: col.editable && record.isChecked,
                dataIndex: col.dataIndex,
                title: col.title,
                handleSave,
            }),
        };
    });

    return (
        <Table
            loading={props?.brochureStore?.isLoadingGoods}
            columns={cols}
            rowSelection={{
                type: "checkbox",
                ...rowSelection,
            }}
            components={components}
            rowClassName={() => "editable-row"}
            dataSource={goods}
            pagination={{pageSize: 5, showSizeChanger: false}}
            size={"small"}
        />
    );
}));

export default GoodsTableComponent;