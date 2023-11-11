import React, {useEffect} from "react";
import {Layout, Space, Spin, Table} from "antd";
import {Content, Header} from "antd/es/layout/layout";
import {BaseStoreInjector} from "../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../styles/Tabs/DistributionsTab.css";
import CreateDistributionButtonComponent from "../Operations/DistributionOperations/CreateDistributionButtonComponent";
import {DistributionStore} from "../../stores/DistributionStore";
import {SHOULD_USE_ONLY_DB_DATA} from "../../constants/Routes";
import NoDataComponent from "../NoDataComponent";
import {DistributionDbProps} from "../../types/DistributionTypes";
import DeleteDistributionButtonComponent from "../Operations/DistributionOperations/DeleteDistributionButtonComponent";

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
            dataIndex: "genderName",
            key: "distributions_table_gender",
        },
        {
            title: "Населённый пункт",
            dataIndex: "townName",
            key: "distributions_table_town",
            width: 315
        },
        {
            title: "Возрастная группа",
            dataIndex: "ageGroupName",
            key: "distributions_table_age_group",
            width: 315
        },
        {
            title: "Число каталогов",
            dataIndex: "brochureCount",
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
                        <DeleteDistributionButtonComponent row={row}/>
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
        props.distributionStore?.updateBrochureDistributions(id);
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
                            dataSource={rows as DistributionDbProps[]}
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