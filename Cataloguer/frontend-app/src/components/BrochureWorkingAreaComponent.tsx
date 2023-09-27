import React, {useState} from "react";
import {Layout, Tabs} from "antd";
import type { TabsProps } from 'antd';
import {Content, Header} from "antd/es/layout/layout";
import "../styles/BrochureWorkingArea.css"
import BrochureDescriptionComponent from "./BrochureDescriptionComponent";
import DistributionDescriptionComponent from "./DistributionDescriptionComponent";

/**
 * Перечисление для вкладок.
 * @param BROCHURE_TAB Вкладка каталога.
 * @param DISTRIBUTION_TAB Вкладка рассылки.
 */
enum TabKeys {
    BROCHURE_TAB = "brochure_description_tab",
    DISTRIBUTION_TAB = "distribution_description_tab",
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
        key: TabKeys.DISTRIBUTION_TAB,
        label: 'Рассылка',
    },
];

/**
 * Компонент рабочей области каталога.
 */
const BrochureWorkingAreaComponent: React.FC = () => {
    /**
     * Ключ текущей (выбранной) вкладки.
     */
    const [currentTabKey, setCurrentTabKey] = useState<string>(TabKeys.BROCHURE_TAB);

    /**
     * Измемняет ключ текущей вкладки.
     * @param tabKey Ключ вкладки.
     */
    const onTabChange = (tabKey: string) => {
        setCurrentTabKey(tabKey);
    };

    /**
     * Возвращает компонент для соответвующей вкладки.
     */
    const getTabContent = () => {
        switch (currentTabKey) {
            case TabKeys.BROCHURE_TAB: return (<BrochureDescriptionComponent/>);
            case TabKeys.DISTRIBUTION_TAB: return (<DistributionDescriptionComponent/>);
            default: return null;
        }
    };

    return (
        <Layout>
            <Header className={"brochure-area-header-style"}>
                <Tabs onTabClick={onTabChange} items={tabs.map(tab => tab)}/>
            </Header>
            <Content className={"brochure-area-content-style"}>
                {getTabContent()}
            </Content>
        </Layout>
    );
};

export default BrochureWorkingAreaComponent;