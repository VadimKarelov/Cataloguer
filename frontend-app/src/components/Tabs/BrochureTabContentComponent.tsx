import {Content, Header} from "antd/es/layout/layout";
import {Layout, Space} from "antd";
import CreateBrochureButtonComponent, {
    ButtonModes
} from "../Operations/BrochureOperations/CreateBrochureButtonComponent";
import DeleteBrochureButtonComponent from "../Operations/BrochureOperations/DeleteBrochureButtonComponent";
import CheckEfficiencyButtonComponent from "../Operations/BrochureOperations/CheckEfficiencyButtonComponent";
import ReleaseBrochureButtonComponent from "../Operations/BrochureOperations/ReleaseBrochureButtonComponent";
import BrochureComponent from "../BrochureComponent";
import React from "react";

/**
 * Содержимое вкладки с каталогами.
 */
const BrochureTabContent: React.FC = () => {
    return (
        <>
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
        </>
    );
};

export default BrochureTabContent;