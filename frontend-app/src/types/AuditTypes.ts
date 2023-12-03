
export interface AuditDbProps {
    id: number,
    dateTime: string,
    typeName: string,
    entityId: number,
    propertyName: string,
    previousValue: string | null,
    newValue: string | null,
}