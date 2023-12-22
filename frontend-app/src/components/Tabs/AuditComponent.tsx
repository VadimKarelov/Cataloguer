import React from "react";
import AuditStore from "../../stores/AuditStore";
import {inject, observer} from "mobx-react";
import {Spin, Table} from "antd";
import {sorter} from "../../Utils";
import {AuditProps} from "../../types/AuditTypes";

/**
 * Столбцы таблицы аудита.
 */
const AUDIT_COLUMNS = [
    {key: "audit_table_dateTime", dataIndex: "dateTime", title: "Дата", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "dateTime")},
    {key: "audit_table_typeName", dataIndex: "typeName", title: "Название типа", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "typeName")},
    {key: "audit_table_entityId", dataIndex: "entityId", title: "Идентификатор сущности", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "entityId")},
    {key: "audit_table_propertyName", dataIndex: "propertyName", title: "Название свойства", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "propertyName")},
    {key: "audit_table_previousValue", dataIndex: "previousValue", title: "Прошлое значение", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "previousValue")},
    {key: "audit_table_newValue", dataIndex: "newValue", title: "Новое значение", sorter: (a: AuditProps, b: AuditProps) => sorter(a, b, "newValue")},
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