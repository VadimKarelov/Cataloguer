import '../styles/App.css';
import React from "react";
import {Layout, Space, Typography} from "antd";
import {Content, Header} from 'antd/es/layout/layout';
import BrochureComponent from "./BrochureComponent";
import CreateBrochureButtonComponent, {
    ButtonModes
} from "./Operations/BrochureOperations/CreateBrochureButtonComponent";
import DeleteBrochureButtonComponent from "./Operations/BrochureOperations/DeleteBrochureButtonComponent";
import CheckEfficiencyButtonComponent from "./Operations/BrochureOperations/CheckEfficiencyButtonComponent";
import ReleaseBrochureButtonComponent from "./Operations/BrochureOperations/ReleaseBrochureButtonComponent";

/**
 * Стартовый компонент.
 */
const App = () => {
    return (
        <Layout className={"white-background-style"}>
            <Header className={"main-window-header"}>
                <Typography.Title className={"header-title"}>
                    Новый тетс эффективного каталога товаров
                </Typography.Title>
            </Header>
            <Layout className={"main-window-layout"}>
                <Header className={"main-window-buttons-panel-style"}>
                    <div className={"buttons-style"}>
                        <Space>
                            <CreateBrochureButtonComponent mode={ButtonModes.CREATE}/>
                            <CreateBrochureButtonComponent mode={ButtonModes.EDIT}/>
                            <DeleteBrochureButtonComponent/>
                            <CheckEfficiencyButtonComponent/>
                            <ReleaseBrochureButtonComponent/>
                        </Space>
                    </div>
                </Header>
                <Layout className={"white-background-style"}>
                    <Content className={"main-window-content white-background-style"}>
                        <BrochureComponent/>
                    </Content>
                </Layout>
            </Layout>
        </Layout>
    );
};

export default App;
