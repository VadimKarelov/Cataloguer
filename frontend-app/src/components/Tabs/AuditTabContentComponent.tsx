import {Header} from "antd/es/layout/layout";
import {Button, Layout} from "antd";
import React, {useEffect, useState} from "react";
import AuditComponent from "./AuditComponent";
import {inject, observer} from "mobx-react";
import AuditStore from "../../stores/AuditStore";

/**
 * Свойства компонента вкладки с аудитом.
 */
interface AuditTabContentComponentProps {
    auditStore?: AuditStore;
}

/**
 * Содержимое вкладки с аудитом.
 */
const AuditTabContentComponent: React.FC<AuditTabContentComponentProps> = inject("auditStore")(observer((props) => {
    /**
     * Счётчик нажатий на кнопку.
     */
    const [clickCount, setClickCount] = useState(0);

    /**
     * Выключена кнопка или нет.
     */
    const [isButtonDisabled, setIsButtonDisabled] = useState(false);

    /**
     * Максимальное число нажитий на кнопку.
     */
    const maxClickCount = 2;

    useEffect(() => {
        if (clickCount >= maxClickCount) {
            setIsButtonDisabled(true);
        }
    }, [clickCount]);

    /**
     * Срабатывает при нажатии на кнопку Обновить.
     */
    const onClick = () => {
        setClickCount(prev => prev + 1);
        if (clickCount < maxClickCount) {
            props.auditStore?.updateCurrentLogs();
            return;
        }
    };

    /**
     * Вызывает запрос на обновление логов.
     */
    useEffect(() => {
        props.auditStore?.updateCurrentLogs();
    }, []);

    return (
        <>
            <Header className={"main-window-buttons-panel-style"}>
                <div className={"buttons-style"}>
                    <Button disabled={isButtonDisabled} onClick={onClick}>Обновить</Button>
                </div>
            </Header>
            <Layout className={"white-background-style"}>
                <AuditComponent/>
            </Layout>
        </>
    );
}));

export default AuditTabContentComponent;