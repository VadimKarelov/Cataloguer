import BaseButton from "../BaseButtonComponent";
import React from "react";
import {DatePicker, Form, Input} from "antd";
import {BaseStoreInjector, EditBrochureHandlerProps} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import GoodsTableComponent from "./EditableTable/GoodsTableComponent";
import moment from "moment";

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
    isRequired: boolean,
    min?: number,
    max?: number,
    defaultValue?: string,
    helpText?: string,
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
interface CreateBrochureButtonComponentProps extends BaseStoreInjector {
    mode: ButtonModes,
}

/**
 * Компонент кнопки Создать/Изменить, открывающий свою модалку.
 */
const CreateBrochureButtonComponent: React.FC<CreateBrochureButtonComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Метаданные модалки редактирования каталога.
     */
    const editFormMetadata: Readonly<MetadataProps[]> = [
        { id: "brochure_name", name: "Название", type: MetadataTypes.STR_FIELD, isRequired: true, min: 1, max: 30},
        { id: "brochure_date", name: "Период выпуска каталога", type: MetadataTypes.DATE_FIELD, isRequired: true, defaultValue: new Date().toDateString()},
        { id: "brochure_edition", name: "Тираж", type: MetadataTypes.NMBR_FIELD, isRequired: true, min: 1, max: Number.MAX_VALUE, defaultValue: "1"},
    ];

    /**
     * Метаданные создания каталога.
     */
    const createFormMetadata: Readonly<MetadataProps[]> = [
        ...editFormMetadata,
        { id: "brochure_positions", name: "Перечень товаров", type: MetadataTypes.TBL_FIELD, isRequired: false},
    ];

    /**
     * Текущий режим.
     */
    const currentMode = modes.get(props.mode) ?? "";

    /**
     * Указатель на форму.
     */
    const [form] = Form.useForm();

    /**
     * Возвращает компонет элемента формы.
     * @param formItem Элемент формы.
     */
    const getFormItemComponent = (formItem: MetadataProps) => {
        switch (formItem.type) {
            case MetadataTypes.NMBR_FIELD: return (<Input type={"number"}/>);
            case MetadataTypes.STR_FIELD: return (<Input/>);
            case MetadataTypes.DATE_FIELD: return (<DatePicker format={"DD.MM.YYYY"}/>);
            case MetadataTypes.TBL_FIELD: return (<GoodsTableComponent/>);
            default: return null;
        }
    };

    /**
     * Возвращает результаты проверки полей.
     * @param _ Служебное поле.
     * @param value Значение поля.
     * @param itemMetadata Метаданные для поля.
     */
    const getValidator = (_: any, value: any, itemMetadata: MetadataProps) => {
        if (!value && itemMetadata.isRequired) return Promise.reject();

        let isFine = true;
        switch (itemMetadata.type) {
            case MetadataTypes.NMBR_FIELD: {
                if (isNaN(parseFloat(value))) return Promise.reject();

                const num = parseFloat(value);
                if (itemMetadata.min) {
                    isFine = num >= itemMetadata.min;
                }

                if (itemMetadata.max) {
                    isFine = num <= itemMetadata.max;
                }
            } break;
            case MetadataTypes.STR_FIELD: {
                const str: string = value;
                const length = str.length;
                if (itemMetadata.min) {
                    isFine = length >= itemMetadata.min;
                }

                if (itemMetadata.max) {
                    isFine = length <= itemMetadata.max;
                }
            } break;
        }

        return isFine ? Promise.resolve() : Promise.reject();
    };

    /**
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
        const metadata = props.mode === ButtonModes.EDIT ? editFormMetadata : createFormMetadata;
        const formId = `brochure_${props.mode === ButtonModes.EDIT ? "edit" : "create"}_form`;
        return (
            <Form id={formId} form={form} name={formId} layout={"vertical"} colon={false}>
                {metadata.map(formItem => {
                    const formItemName: Readonly<string> = formItem.id.slice(formItem.id.indexOf('_') + 1);
                    const defaultValue = formItemName === "date" ? moment(formItem.defaultValue) : formItem.defaultValue;
                    return (
                        <Form.Item
                            key={formItem.id}
                            label={formItem.name}
                            name={formItemName}
                            initialValue={defaultValue}
                            rules={[{
                                required: formItem.isRequired,
                                validator: (_, value) => getValidator(_, value, formItem),
                            }]}
                        >
                            {getFormItemComponent(formItem)}
                        </Form.Item>
                    );
                })}
            </Form>
        );
    };

    /**
     * Обработчик создания/редактирования каталога.
     * @param values Значения полей.
     */
    const handleDbAction = (values: any) => {
        values.date = moment(values?.date).format();
        values.edition = parseFloat(values.edition);

        if (props.mode === ButtonModes.CREATE) {
            props.brochureStore?.handleCreateBrochure(values);
        } else {
            const brochureEditProps: EditBrochureHandlerProps = {
                name: values.name,
                date: values.date,
                edition: values.edition
            };
            props.brochureStore?.handleEditBrochure(brochureEditProps);
        }

        form.resetFields();
    };

    /**
     * Обрабатывает нажатие кнопки сохранить.
     */
    const onOkClick = (): Promise<void> => {
        return form.validateFields()
                                .then(
                                    (values) => {
                                        handleDbAction(values);
                                        return Promise.resolve();
                                    },
                                    (info) => {
                                        console.log('Validate Failed:', info);
                                        return Promise.reject();
                                    }
                                );
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: `${props.mode === ButtonModes.EDIT ? "Редактировать" : "Создать"} каталог`,
        form: form,
        okText: "Сохранить",
        cancelText: "Отменить",
        onOkClick: onOkClick,
        children: getForm(),
    };

    /**
     * Заполняет поля данными текалога.
     */
    const fillFieldsOnEdit = (): void => {
        if (currentBrochure === null) return;

        form.setFieldsValue({
            id: currentBrochure.id,
            name: currentBrochure.name,
            date: moment(currentBrochure.creationDate),
            edition: currentBrochure?.edition,
        });
    };

    /**
     * Срабатывает при нажатии кнопки.
     * Используется в качестве callback функции в родительском компоненте.
     */
    const onClick = () => {
        props.brochureStore?.updateGoodsList();
        props.mode === ButtonModes.EDIT && fillFieldsOnEdit();
    };

    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Свойства кнопки для родительского компонента.
     */
    const buttonProps = {
        buttonText: currentMode,
        onClick: onClick,
        isDisabled: currentBrochure === null && props.mode === ButtonModes.EDIT,
    };

    return (
        <BaseButton
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CreateBrochureButtonComponent;