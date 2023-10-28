import React, {useEffect, useState} from "react";
import {Layout, Tabs, Typography} from "antd";
import type { TabsProps } from 'antd';
import {Content, Header} from "antd/es/layout/layout";
import "../styles/BrochureWorkingArea.css"
import BrochureDescriptionComponent from "./Tabs/BrochureDescriptionComponent";
import DistributionDescriptionComponent from "./Tabs/DistributionDescriptionComponent";
import GoodsDescriptionComponent from "./Tabs/GoodsDescriptionComponent";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector, BrochureProps} from "../types/BrochureTypes";

/**
 * Перечисление для вкладок.
 * @param BROCHURE_TAB Вкладка каталога.
 * @param DISTRIBUTION_TAB Вкладка рассылки.
 */
enum TabKeys {
    BROCHURE_TAB = "brochure_description_tab",
    DISTRIBUTION_TAB = "distribution_description_tab",
    GOODS_TAB = "goods_description_tab",
}

/**
 * Множество вкладок.
 */
const tabs: Readonly<TabsProps["items"]> = [
    {
        key: TabKeys.BROCHURE_TAB,
        label: "Каталог",
    },
    {
        key: TabKeys.GOODS_TAB,
        label: 'Состав',
    },
    {
        key: TabKeys.DISTRIBUTION_TAB,
        label: 'Рассылка',
    },
];

/**
 * Свойства компонента BrochureWorkingAreaComponent.
 */
interface BrochureWorkingAreaComponentProps extends BaseStoreInjector {
}

/**
 * Переменная для получения вкладки из хранилища
 */
const SS_SAVED_TAB: Readonly<string> = "ss_saved_tab";

/**
 * Компонент рабочей области каталога.
 */
const BrochureWorkingAreaComponent: React.FC<BrochureWorkingAreaComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const [brochure, setBrochure] = useState<BrochureProps | null>(null);

    /**
     * Ключ текущей (выбранной) вкладки.
     */
    const [currentTabKey, setCurrentTabKey] = useState<string>(TabKeys.BROCHURE_TAB);

    /**
     * Хук для изменения текущей вкладки.
     * При обновлении страницы подставляет выбранную вкладку.
     * При смене каталога устанавливает первую вкладку.
     */
    useEffect(() =>
        setBrochure(prevBrochure => {
            let key: string = TabKeys.BROCHURE_TAB;

            const prevId = brochure !== null ? brochure.id : -1;
            const currentBrochure = props.brochureStore?.currentBrochure ?? null;

            if (currentBrochure) {
                const currentId = currentBrochure.id;
                if (currentId !== prevId && prevId === -1) {
                    const oldTab = sessionStorage.getItem(SS_SAVED_TAB);
                    key = oldTab !== null ? oldTab : TabKeys.BROCHURE_TAB;
                }

            }

            setCurrentTabKey(key);
            return currentBrochure;
    }), [props.brochureStore?.currentBrochure?.id]);

    /**
     * Измемняет ключ текущей вкладки.
     * @param tabKey Ключ вкладки.
     */
    const onTabChange = (tabKey: string): void => {
        setCurrentTabKey(tabKey);
        sessionStorage.setItem(SS_SAVED_TAB, tabKey);
    };

    /**
     * Возвращает компонент для соответвующей вкладки.
     */
    const getTabContent = () => {
        switch (currentTabKey) {
            case TabKeys.BROCHURE_TAB: return (<BrochureDescriptionComponent/>);
            case TabKeys.DISTRIBUTION_TAB: return (<DistributionDescriptionComponent/>);
            case TabKeys.GOODS_TAB: return (<GoodsDescriptionComponent/>);
            default: return null;
        }
    };

    /**
     * Возвращает базовую информацию для строки над табами.
     */
    const getBrochureBaseInfo = (): string => {
        if (brochure === null) return "";

        const {name, edition, status} = brochure;
        return `${name} \\ ${edition} \\ ${status}`;
    };

    return (
        <Layout className={"white-background-style"}>
            <Header className={"brochure-area-header-style"}>
                <Typography.Paragraph className={"paragraph-header-style"}>
                    {getBrochureBaseInfo()}
                </Typography.Paragraph>
                <Tabs activeKey={currentTabKey} onTabClick={onTabChange} items={tabs.map(tab => tab)}/>
            </Header>
            <Content className={"brochure-area-content-style"}>
                {getTabContent()}
            </Content>
        </Layout>
    );
}));

export default BrochureWorkingAreaComponent;