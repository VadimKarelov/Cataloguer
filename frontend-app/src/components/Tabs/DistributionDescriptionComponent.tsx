import React, {useEffect} from "react";
import {Button, Layout, Popconfirm, Space, Spin, Table} from "antd";
import {Content, Header} from "antd/es/layout/layout";
import {BaseStoreInjector} from "../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../styles/Tabs/DistributionsTab.css";
import CreateDistributionButtonComponent from "../Operations/DistributionOperations/CreateDistributionButtonComponent";
import {DistributionStore} from "../../stores/DistributionStore";
import {SHOULD_USE_ONLY_DB_DATA} from "../../constants/Routes";
import NoDataComponent from "../NoDataComponent";

/**
 * Свойства компонента GoodsDescriptionComponent.
 * @param distributionStore Хранилище данных о рассылках.
 */
interface DistributionDescriptionComponentProps extends BaseStoreInjector {
    distributionStore?: DistributionStore,
}

/**
 * Компонент описания рассылки.
 */
const DistributionDescriptionComponent: React.FC<DistributionDescriptionComponentProps> = inject("brochureStore", "distributionStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const brochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Строки таблицы.
     */
    const rows = SHOULD_USE_ONLY_DB_DATA ? (props.distributionStore?.distributions ?? []) : (brochure?.distributions ?? []);

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
            render: (_: any, row: any) => {
                return (
                    <Space>
                        <CreateDistributionButtonComponent row={row}/>
                        <Popconfirm title={`Удалить рассылку?`} okText={"Удалить"} cancelText={"Отменить"} /*onConfirm={() => handleDelete(record.key)}*/>
                            <Button>Удалить</Button>
                        </Popconfirm>
                    </Space>
                );
            },
            width: 110
        },
    ];

    /**
     * Обновляет список рассылок.
     */
    const updateDistributions = () => {
        const id = brochure?.id ?? -1;
        if (id === -1) return;
        props.distributionStore?.updateBrochureDistributions(id)
    };

    /**
     * Вызывает метод обновления рассылок.
     */
    useEffect(updateDistributions, [props.brochureStore?.currentBrochure?.id]);

    return (
        brochure !== null ? (
            <Layout>
                <Header className={"distributions-tab-header-style"}>
                    <CreateDistributionButtonComponent/>
                </Header>
                <Content className={"distributions-tab-content-style"}>
                    <Spin spinning={props.distributionStore?.isLoadingDistributions} size={"large"}>
                        {hasData ? (<Table
                            className={"distributions-table-style"}
                            size={"middle"}
                            scroll={{y: "calc(100vh - 278px)", x: "max-content"}}
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

export default DistributionDescriptionComponent;