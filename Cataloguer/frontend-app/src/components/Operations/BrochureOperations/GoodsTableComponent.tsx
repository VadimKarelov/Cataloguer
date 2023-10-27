import {Table} from "antd";
import React from "react";
import {BaseStoreInjector, GoodsProps} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";

/**
 * Коллекция свойств столбцов таблицы.
 */
const columns = [
    {
        title: "Название",
        dataIndex: "name",
        key: "goods_table_name",
        // width: 315
    },
    {
        title: "Цена",
        dataIndex: "price",
        key: "goods_table_cost",
        // width: 128
    },
];

/**
 * Свойства, необходимые для обработки выделяемых строк по checkbox.
 */
const rowSelection = {
    onChange: (selectedRowKeys: React.Key[], selectedRows: GoodsProps[]) => {
        console.log(selectedRowKeys)
        console.log(selectedRows)
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: (record: any) => ({
        // disabled: record.name === 'Disabled User', // Column configuration not to be checked
        id: record.id,
        name: record.name,
        price: record.price
    }),
};

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
     * Все возможные товары.
     */
    const allGoods = props.brochureStore?.allGoods ?? [];

    return (
        <Table
            columns={columns}
            rowSelection={{
                type: 'checkbox',
                ...rowSelection,
            }}
            dataSource={allGoods.map(good => ({...good, key: `good_${good.id}`}))}
            pagination={{pageSize: 5}}
            size={"small"}
        />
    );
}));

export default GoodsTableComponent;