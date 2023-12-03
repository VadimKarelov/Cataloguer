import React, {useEffect} from "react";
import AuditStore from "../../stores/AuditStore";
import {inject, observer} from "mobx-react";
import {Spin, Table} from "antd";

/**
 * Столбцы таблицы аудита.
 */
const AUDIT_COLUMNS: Readonly<{key: string, dataIndex: string, title: string}[]> = [
    {key: "audit_table_dateTime", dataIndex: "dateTime", title: "Дата"},
    {key: "audit_table_typeName", dataIndex: "typeName", title: "Название типа"},
    {key: "audit_table_entityId", dataIndex: "entityId", title: "Идентификатор сущности"},
    {key: "audit_table_propertyName", dataIndex: "propertyName", title: "Название свойства"},
    {key: "audit_table_previousValue", dataIndex: "previousValue", title: "Прошлое значение"},
    {key: "audit_table_newValue", dataIndex: "newValue", title: "Новое значение"},
];

/**
 * Свойства компонента аудита.
 */
interface AuditComponentProps {
    auditStore?: AuditStore,
}

/**
 * Компонент аудита.
 */
const AuditComponent: React.FC<AuditComponentProps> = inject("auditStore")(observer((props) => {
    /**
     * Логи.
     */
    const logs = props.auditStore?.logs ?? [];

    /**
     * Вызывает запрос на обновление логов.
     */
    useEffect(() => {
        props.auditStore?.updateCurrentLogs();
    }, []);

    return (
        <Spin size={"large"} spinning={props.auditStore?.isLoadingLogs}>
            <Table
                scroll={{y: "calc(100vh - 170px)"}}
                columns={AUDIT_COLUMNS.map(col => col)}
                dataSource={logs.map(log => ({key: `log_${log.id}`, ...log}))}
                pagination={false}
            />
        </Spin>
    );
}));

export default AuditComponent;