import '../styles/App.css';
import React from "react";
import {Button, Layout, Space, Typography} from "antd";
import {Content, Header} from 'antd/es/layout/layout';
import Sider from "antd/es/layout/Sider";
import BrochureMenuComponent from "./BrochureMenuComponent";
import BrochureComponent from "./BrochureComponent";

/**
 * Стартовый компонент.
 */
const App = () => {
    return (
        <Layout>
            <Header className={"main-window-header"}>
                <Typography.Title className={"header-title"}>
                    Формирование эффективного каталога товаров
                </Typography.Title>
            </Header>
            <Layout className={"main-window-layout"}>
                <Header className={"main-window-buttons-panel-style"}>
                    <div className={"buttons-style"}>
                        <Space>
                            <Button>Создать</Button>
                            <Button>Изменить</Button>
                            <Button>Удалить</Button>
                            <Button>Проверить</Button>
                        </Space>
                    </div>
                </Header>
                <Layout>
                    <Sider width={350} className={"sider-style"}>
                        <BrochureMenuComponent/>
                    </Sider>
                    <Content className={"main-window-content"}>
                        <BrochureComponent/>
                    </Content>
                </Layout>
            </Layout>
        </Layout>
    );
};

export default App;
