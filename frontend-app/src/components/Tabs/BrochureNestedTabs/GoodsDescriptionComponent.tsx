import {Layout, Space, Spin, Table} from "antd";
import React, {useEffect} from "react";
import {BaseStoreInjector, customProp, GoodProps} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../../styles/Tabs/GoodsTab.css";
import {Content, Header} from "antd/es/layout/layout";
import GoodsStore from "../../../stores/GoodsStore";
import {SHOULD_USE_ONLY_DB_DATA} from "../../../constants/Routes";
import NoDataComponent from "../../NoDataComponent";
import CreateBrochureButtonComponent, {
    ButtonModes
} from "../../Operations/BrochureOperations/CreateBrochureButtonComponent";
import DeleteGoodsButtonComponent from "../../Operations/GoodsOperations/DeleteGoodsButtonComponent";

/**
 * Свойства компонента GoodsDescriptionComponent.
 * @param goodsStore Хранилище данных о товарах.
 */
interface GoodsDescriptionComponentProps extends BaseStoreInjector {
    goodsStore?: GoodsStore,
}

/**
 * Компонент описания каталога.
 */
const GoodsDescriptionComponent: React.FC<GoodsDescriptionComponentProps> = inject("brochureStore", "goodsStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Строки таблицы.
     */
    const rows = ((SHOULD_USE_ONLY_DB_DATA ?
                                    (props.goodsStore?.goods ?? []).map(good => ({key: `good_${good.id}`, ...good}))
                                : brochure?.goods) ?? []).map((good, i) => ({key: `good_${i}`, ...good}));

    /**
     * Есть данные или нет.
     */
    const hasData = brochure !== null && rows.length > 0;

    /**
     * Сортирует столбец таблицы.
     * @param a Строка 1.
     * @param b Строка 2.
     * @param key Свойство, по которому нужно сортировать.
     */
    const sorter = (a: GoodProps, b: GoodProps, key: string) => {
        const f: customProp = a, s: customProp = b;
        const first = f[key], second = s[key];

        if (typeof first === "string") return first.localeCompare(second);
        if (typeof first === "number") return first - second;

        return 0;
    };

    /**
     * Колонки таблицы.
     */
    const columns = [
        {
            title: "Наименование",
            dataIndex: "name",
            key: "goods_table_name",
            sorter: (a: GoodProps, b: GoodProps) => sorter(a, b, "name")
        },
        {
            title: "Цена, руб",
            dataIndex: "price",
            key: "goods_table_price",
            sorter: (a: GoodProps, b: GoodProps) => sorter(a, b, "price")
        },
        {
            title: "Операция",
            dataIndex: "operation",
            key: "goods_table_operation",
            width: 110,
            render: (_: any, row: any) => (
                <Space>
                    <DeleteGoodsButtonComponent row={row}/>
                </Space>
            ),
        },
    ];

    /**
     * Обновляет список товаров.
     */
    const updateGoods = (): void => {
        const id = brochure?.id ?? -1;
        if (id === -1) return;
        props.goodsStore?.updateCurrentBrochureGoods(id);
    };

    /**
     * Вызывает метод обновления товаров.
     */
    useEffect(updateGoods, [props.brochureStore?.currentBrochure?.id]);

    return (
        brochure !== null ? (
            <Layout>
                <Header className={"goods-tab-header-style"}>
                    <Space>
                        <CreateBrochureButtonComponent mode={ButtonModes.CREATE_GOODS}/>
                    </Space>
                </Header>
                <Content className={"goods-tab-content-style"}>
                    <Spin spinning={props.goodsStore?.isLoadingGoods} size={"large"}>
                        {hasData ? (<Table
                            className={"goods-table-style"}
                            size={"middle"}
                            scroll={{y: "calc(100vh - 283px)", x: "max-content"}}
                            columns={columns}
                            dataSource={rows}
                            pagination={false}
                        />) : (
                            <NoDataComponent/>
                        )}
                    </Spin>
                </Content>
            </Layout>
        ) : (
            <NoDataComponent/>
        )
    );
}));

export default GoodsDescriptionComponent;