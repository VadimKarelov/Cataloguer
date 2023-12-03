import '../styles/App.css';
import React, {useState} from "react";
import {Layout, Tabs, Typography} from "antd";
import {Header} from 'antd/es/layout/layout';
import BrochureTabContent from './Tabs/BrochureTabContentComponent';
import AuditTabContentComponent from "./Tabs/AuditTabContentComponent";
import {TabProps} from "../types/AppTypes";

/**
 * Перечисление табов.
 */
enum TabKey {
    BROCHURES = "main_tab_brochures",
    AUDIT = "main_tab_loggs",
}

/**
 * Вкладки меню первого уровня.
 */
const TABS: Readonly<TabProps[]> = [
    {label: "Каталоги", key: TabKey.BROCHURES},
    {label: "Аудит", key: TabKey.AUDIT}
];

/**
 * Стартовый компонент.
 */
const App = () => {
    /**
     * Текущая вкладка.
     */
    const [currentTab, setCurrentTab] = useState<string>(TabKey.BROCHURES);

    /**
     * Содержимая вкладки.
     */
    const getTabContent = () => {
        switch (currentTab) {
            case TabKey.BROCHURES: return (<BrochureTabContent/>);
            case TabKey.AUDIT: return (<AuditTabContentComponent/>);
            default: return null;
        }
    };

    return (
        <Layout className={"white-background-style"}>
            <Header className={"main-window-header"}>
                <Typography.Title className={"header-title"}>
                    Формирование эффективного каталога товаров
                </Typography.Title>
                <Tabs
                    className={"main-tabs-style"}
                    activeKey={currentTab}
                    onChange={setCurrentTab}
                    items={TABS.map(tab => tab)}
                />
            </Header>
            <Layout className={"main-window-layout"}>
                {getTabContent()}
            </Layout>
        </Layout>
    );
};

export default App;
