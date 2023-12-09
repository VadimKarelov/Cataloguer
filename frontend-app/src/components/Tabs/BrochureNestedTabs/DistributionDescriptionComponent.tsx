import React, {useEffect, useState} from "react";
import {Layout, Space, Spin, Table} from "antd";
import {Content, Header} from "antd/es/layout/layout";
import {BaseStoreInjector, customProp} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import "../../../styles/Tabs/DistributionsTab.css";
import CreateDistributionButtonComponent from "../../Operations/DistributionOperations/CreateDistributionButtonComponent";
import {DistributionStore} from "../../../stores/DistributionStore";
import NoDataComponent from "../../NoDataComponent";
import {DistributionDbProps} from "../../../types/DistributionTypes";
import DeleteDistributionButtonComponent from "../../Operations/DistributionOperations/DeleteDistributionButtonComponent";
import {sorter} from "../../../Utils";
import {SHOULD_USE_ONLY_DB_DATA} from "../../../constants/EnvironmentVariables";
import Search from "antd/es/input/Search";

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
    const rows = SHOULD_USE_ONLY_DB_DATA ?
        (props.distributionStore?.distributions ?? []).map(distr => ({key: `distribution_${distr.id}`, ...distr}))
        : (brochure?.distributions ?? []).map((distr, i) => ({key: `distribution_${i}`, ...distr}));

    /**
     * Есть данные или нет.
     */
    const hasData = brochure !== null && rows.length > 0;

    /**
     * Искомое значение для поиска.
     */
    const [searchValue, setSearchValue] = useState<string>("");

    /**
     * Колонки таблицы.
     */
    const columns = [
        {
            title: "Пол",
            dataIndex: "genderName",
            key: "distributions_table_gender",
            sorter: (a: DistributionDbProps, b: DistributionDbProps) => sorter(a, b, "genderName")
        },
        {
            title: "Населённый пункт",
            dataIndex: "townName",
            key: "distributions_table_town",
            width: 315,
            sorter: (a: DistributionDbProps, b: DistributionDbProps) => sorter(a, b, "townName")
        },
        {
            title: "Возрастная группа",
            dataIndex: "ageGroupName",
            key: "distributions_table_age_group",
            width: 290,
            sorter: (a: DistributionDbProps, b: DistributionDbProps) => sorter(a, b, "ageGroupName")
        },
        {
            title: "Число каталогов",
            dataIndex: "brochureCount",
            key: "distributions_table_count",
            width: 150,
            sorter: (a: DistributionDbProps, b: DistributionDbProps) => sorter(a, b, "brochureCount")
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

    /**
     * Фильтрует таблицу.
     * @param searchValue Искомое значение.
     */

    const onTableSearch = (searchValue: string) => {
        setSearchValue(searchValue.trim().toLowerCase());
    };

    /**
     * Возвращает фильрованные рассылки.
     */
    const getFilteredDistributions = () => {
        return (rows as DistributionDbProps[]).filter(row => {
            const currentRow: customProp = row;
            return searchValue === "" || Object.keys(row).some(key => {
                const val = currentRow[key];
                return val.toString().toLowerCase().includes(searchValue);
            });
        });
    };

    return (
        brochure !== null ? (
            <Layout>
                <Header className={"distributions-tab-header-style"}>
                    <CreateDistributionButtonComponent/>
                </Header>
                <Content className={"distributions-tab-content-style"}>
                    <Search
                        className={"distributions-tab-search"}
                        size={"large"}
                        onSearch={onTableSearch}
                        allowClear
                        title={"Найти рассылку"}
                        disabled={!hasData}
                    />
                    <Spin spinning={props.distributionStore?.isLoadingDistributions} size={"large"}>
                        {hasData ? (<Table
                            className={"distributions-table-style"}
                            size={"middle"}
                            scroll={{y: "calc(100vh - 340px)", x: "max-content"}}
                            columns={columns}
                            dataSource={getFilteredDistributions()}
                            pagination={false}
                            bordered
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