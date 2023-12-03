import {Header} from "antd/es/layout/layout";
import {Button, Layout} from "antd";
import React from "react";
import AuditComponent from "./AuditComponent";

/**
 * Свойства компонента вкладки с аудитом.
 */
interface AuditTabContentComponentProps {
}

/**
 * Содержимое вкладки с аудитом.
 */
const AuditTabContentComponent: React.FC<AuditTabContentComponentProps> = (props) => {
    return (
        <>
            <Header className={"main-window-buttons-panel-style"}>
                <div className={"buttons-style"}>
                    <Button disabled={true}>Обновить</Button>
                </div>
            </Header>
            <Layout className={"white-background-style"}>
                <AuditComponent/>
            </Layout>
        </>
    );
};

export default AuditTabContentComponent;