import {Input, Form, InputRef} from "antd";
import React, {useContext, useEffect, useRef, useState} from "react";
import {customProp, GoodsExtendedProps} from "../../../../types/BrochureTypes";
import {EditableContext} from "./EditableRowComponent";

/**
 * Свойства редактируемой ячейки.
 * @param title Название ячейки.
 * @param editable Свойство редактируемости.
 * @param children Дочерний компонент.
 * @param dataIndex Индекс доступа к ячейке.
 * @param record Запись в таблице.
 * @param handleSave Callback функция сохранения изменений в ячейке.
 * @param restProps Остальные свойства.
 */
interface EditableCellProps {
    title: React.ReactNode;
    editable: boolean;
    children: React.ReactNode;
    dataIndex: string;
    record: GoodsExtendedProps;
    handleSave: (record: GoodsExtendedProps) => void;
}

/**
 * Компонент редактируемой ячейки.
 */
export const EditableCellComponent: React.FC<EditableCellProps> = ({
                                                       title,
                                                       editable,
                                                       children,
                                                       dataIndex,
                                                       record,
                                                       handleSave,
                                                       ...restProps
                                                   }) => {
    /**
     * Происходит ли редактирование ячейки.
     */
    const [isEditing, setIsEditing] = useState(false);

    /**
     * Ссылка на редактируемое поле.
     */
    const inputRef = useRef<InputRef>(null);

    /**
     * Указатель на форму (таблицу).
     */
    const form = useContext(EditableContext)!;

    /**
     * Хук, обрабатывающий состояние редактирования.
     */
    useEffect(() => {
        if (isEditing) {
            inputRef.current!.focus();
        }
    }, [isEditing]);

    /**
     * Переключает в режим редактирования.
     */
    const toggleEdit = () => {
        setIsEditing(!isEditing);
        const r: customProp = record;
        form.setFieldsValue({ [dataIndex]: r[dataIndex] });
    };

    /**
     * Сохраняет изменённые данные ячейки.
     */
    const save = async () => {
        try {
            const values = await form.validateFields();

            toggleEdit();
            handleSave({ ...record, ...values });
        } catch (errInfo) {
            console.log('Save failed:', errInfo);
        }
    };

    /**
     * Дочерний компонент.
     */
    let childNode = children;

    /**
     * Валидатор, проверяющий наличие данных в ячейке.
     * @param _ Какое-то служебное поле.
     * @param value Значение объекта в ячейке.
     */
    const getEmptyCheckResult = (_: any, value: any) => {
        if (!value) return Promise.reject();

        return Promise.resolve();
    };

    /**
     * Проверяет входимость значения в адекватный диапазон значений.
     * @param _ Какое-то служебное поле.
     * @param value Значение объекта в ячейке.
     */
    const getBouncesCheck = (_: any, value: any) => {
        if (value < 0) return Promise.reject();
        if (value > Number.MAX_VALUE) return Promise.reject();

        return Promise.resolve();
    };

    if (editable) {
        childNode = isEditing ? (
            <Form.Item
                style={{ margin: 0 }}
                name={dataIndex}
                rules={[
                    {
                        required: true,
                        validator: getEmptyCheckResult,
                        message: `Введите цену`,
                    },
                    {
                        required: false,
                        validator: getBouncesCheck,
                        message: `Цена не приемлема`,
                    },
                ]}
            >
                <Input type={"number"} ref={inputRef} onPressEnter={save} onBlur={save} />
            </Form.Item>
        ) : (
            <div className="editable-cell-value-wrap" style={{ paddingRight: 24 }} onClick={toggleEdit}>
                {children}
            </div>
        );
    }

    return (<td {...restProps}>{childNode}</td>);
};