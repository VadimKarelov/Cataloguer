import React from "react";
import {Button, Layout, Result, Space, Table} from "antd";
import {Content, Header} from "antd/es/layout/layout";
import {NO_DATA_TEXT} from "../../constants/Messages";
import {BaseStoreInjector} from "../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../styles/Tabs/DistributionsTab.css";
import CreateDistributionButtonComponent from "../Operations/DistributionOperations/CreateDistributionButtonComponent";

/**
 * Свойства компонента GoodsDescriptionComponent.
 */
interface DistributionDescriptionComponentProps extends BaseStoreInjector {
}

/**
 * Компонент описания рассылки.
 */
const DistributionDescriptionComponent: React.FC<DistributionDescriptionComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Строки таблицы.
     */
    const rows = brochure?.distributions ?? [];

    /**
     * Есть данные или нет.
     */
    const hasData = brochure !== null && rows.length > 0;

    /**
     * Колонки таблицы.
     */
    const columns = [
        {
            title: "Пол",
            dataIndex: "gender",
            key: "distributions_table_gender",
        },
        {
            title: "Населённый пункт",
            dataIndex: "town",
            key: "distributions_table_town",
            width: 315
        },
        {
            title: "Возрастная группа",
            dataIndex: "ageGroup",
            key: "distributions_table_age_group",
            width: 315
        },
        {
            title: "Число каталогов",
            dataIndex: "count",
            key: "distributions_table_count",
            width: 128
        },
        {
            title: "Операция",
            dataIndex: "operation",
            key: "distributions_table_operation",
            render: () => {
                return (
                    <Space>
                        <Button>Изменить</Button>
                        <Button>Удалить</Button>
                    </Space>
                );
            },
            width: 110
        },
    ];

    return (
        hasData ? (
            <Layout>
                <Header className={"distributions-tab-header-style"}>
                    <CreateDistributionButtonComponent/>
                </Header>
                <Content className={"distributions-tab-content-style"}>
                    <Table
                        className={"distributions-table-style"}
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

export default DistributionDescriptionComponent;