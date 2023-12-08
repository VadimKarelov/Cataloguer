/**
 * Свойства элементов коллекции аудита.
 * @param id Идентификатор.
 * @param dateTime Дата лога.
 * @param typeName Название типа.
 * @param entityId Идентификатор сущности.
 * @param propertyName Название свойства.
 * @param previousValue Предыдущее значение.
 * @param newValue Новое значение.
 */
export interface AuditDbProps {
    id: number,
    dateTime: string,
    typeName: string,
    entityId: number,
    propertyName: string,
    previousValue: string | null,
    newValue: string | null,
}

/**
 * Свойства элементов коллекции аудита.
 * @param key Ключ строки таблицы.
 */
export interface AuditProps extends AuditDbProps {
    key: string,
}