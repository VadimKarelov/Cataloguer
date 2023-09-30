import {Button, Layout, Result, Space, Table} from "antd";
import React from "react";
import {BaseStoreInjector} from "../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../styles/Tabs/GoodsTab.css";
import {Content, Header} from "antd/es/layout/layout";
import {NO_DATA_TEXT} from "../../Messages";

/**
 * Свойства компонента GoodsDescriptionComponent.
 */
interface GoodsDescriptionComponentProps extends BaseStoreInjector {
}

/**
 * Компонент описания каталога.
 */
const GoodsDescriptionComponent: React.FC<GoodsDescriptionComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Строки таблицы.
     */
    const rows = brochure?.goods ?? [];

    /**
     * Есть данные или нет.
     */
    const hasData = brochure !== null && rows.length > 0;

    /**
     * Колонки таблицы.
     */
    const columns = [
        {
            title: "Наименование",
            dataIndex: "name",
            key: "goods_table_name",
        },
        {
            title: "Цена, руб",
            dataIndex: "price",
            key: "goods_table_price",
        },
        {
            title: "Операция",
            dataIndex: "operation",
            key: "goods_table_operation",
            render: () => (<Button>Удалить</Button>),
            width: 110
        },
    ];

    return (
        hasData ? (
            <Layout>
                <Header className={"goods-tab-header-style"}>
                    <Space>
                        <Button>Добавить</Button>
                        <Button>Изменить</Button>
                    </Space>
                </Header>
                <Content className={"goods-tab-content-style"}>
                    <Table
                        className={"goods-table-style"}
                        size={"middle"}
                        scroll={{y: "calc(100vh - 250px)", x: "max-content"}}
                        columns={columns}
                        dataSource={rows}
                        pagination={false}
                    />
                </Content>
            </Layout>
        ) : (
            <Result
                status="error"
                title={NO_DATA_TEXT}
            />
        )
    );
}));

export default GoodsDescriptionComponent;