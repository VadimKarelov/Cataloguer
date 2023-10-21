import {Form, FormInstance} from "antd";
import React from "react";
import "../../../../styles/GoodsTableComponent.css";

/**
 * Редактируемый контекст.
 */
export const EditableContext = React.createContext<FormInstance<any> | null>(null);

/**
 * Свойства редактруемой строчки.
 * @param index Индекс строки.
 */
interface EditableRowProps {
    index: number;
}

/**
 * Компонент для строчки в таблице.
 * @param index - индекс строчки.
 * @param props - передаваемые свойства.
 */
export const EditableRowComponent: React.FC<EditableRowProps> = ({ index, ...props }) => {
    /**
     *  Хук для использования формы.
     */
    const [form] = Form.useForm();

    /**
     * Возвращает строчку, состоящую из ячеек.
     */
    return (
        <Form form={form} component={false}>
            <EditableContext.Provider value={form}>
                <tr {...props} />
            </EditableContext.Provider>
        </Form>
    );
};