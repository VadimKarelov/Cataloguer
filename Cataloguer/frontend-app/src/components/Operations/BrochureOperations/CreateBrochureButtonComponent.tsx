import BaseButton from "../BaseButtonComponent";
import React from "react";
import {DatePicker, Form, Input, Table} from "antd";

/**
 * Перечисления типов в метаданных для компонента CreateBrochureButtonComponent.
 */
export enum MetadataTypes {
    STR_FIELD,
    NMBR_FIELD,
    TBL_FIELD,
    DATE_FIELD,
    LIST_FIELD,
}

/**
 * Свойства метаданных для компонента CreateBrochureButtonComponent.
 */
export interface MetadataProps {
    id: string,
    name: string,
    type: MetadataTypes,
}

/**
 * Режимы кнопок компонента.
 */
export enum ButtonModes {
    CREATE,
    EDIT
}

/**
 * Словарь с режимами.
 */
const modes = new Map([
    [ButtonModes.CREATE, "Создать"],
    [ButtonModes.EDIT, "Изменить"],
]);

/**
 * Свойства компонента CreateBrochureButtonComponent.
 * @param mode Режим работы кнопок компонента.
 */
interface CreateBrochureButtonComponentProps {
    mode: ButtonModes
}

/**
 * Компонент кнопки Создать/Изменить, открывающий свою модалку.
 */
const CreateBrochureButtonComponent: React.FC<CreateBrochureButtonComponentProps> = (props) => {
    /**
     * Метаданные модалки редактирования каталога.
     */
    const editFormMetadata: Readonly<MetadataProps[]> = [
        { id: "brochure_name", name: "Название", type: MetadataTypes.STR_FIELD},
        { id: "brochure_create_date", name: "Период выпуска каталога", type: MetadataTypes.DATE_FIELD},
        { id: "brochure_edition", name: "Тираж", type: MetadataTypes.STR_FIELD},
    ];

    /**
     * Метаданные создания каталога.
     */
    const createFormMetadata: Readonly<MetadataProps[]> = [
        ...editFormMetadata,
        { id: "brochure_goods", name: "Перечень товаров", type: MetadataTypes.TBL_FIELD},
    ];

    /**
     * Текущий режим.
     */
    const currentMode = modes.get(props.mode) ?? "";

    /**
     * Возвращает компонет элемента формы.
     * @param formItem Элемент формы.
     */
    const getFormItemComponent = (formItem: MetadataProps) => {
        switch (formItem.type) {
            case MetadataTypes.NMBR_FIELD:
            case MetadataTypes.STR_FIELD: return (<Input/>);
            case MetadataTypes.DATE_FIELD: return (<DatePicker/>);
            case MetadataTypes.TBL_FIELD: return (<Table/>);
            default: return null;
        }
    };

    /**
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
        const metadata = props.mode === ButtonModes.EDIT ? editFormMetadata : createFormMetadata;
        return (
            <Form layout={"vertical"} colon={false}>
                {metadata.map(formItem => {
                    return (
                        <Form.Item key={formItem.id} label={formItem.name}>
                            {getFormItemComponent(formItem)}
                        </Form.Item>
                    );
                })}
            </Form>
        );
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: props.mode === ButtonModes.EDIT ? "Редактировать каталог" : "Создать каталог",
        okText: "Сохранить",
        cancelText: "Отменить",
        children: getForm(),
    };

    return (
        <BaseButton
            buttonProps={{buttonText: currentMode}}
            modalProps={modalProps}
        />
    );
};

export default CreateBrochureButtonComponent;