import BaseButton from "../BaseButtonComponent";
import React from "react";
import {DatePicker, Form, Input} from "antd";
import {BaseStoreInjector, EditBrochureHandlerProps} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import GoodsTableComponent from "./EditableTable/GoodsTableComponent";
import moment from "moment";
import {getValidator} from "../../../Utils";
import {openNotification} from "../../NotificationComponent";
import GoodsStore from "../../../stores/GoodsStore";
import BrochureService from "../../../services/BrochureService";
import GoodsService from "../../../services/GoodsService";

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
 * @param id Идентификатор поля.
 * @param name Наименование поля.
 * @param type Тип поля.
 * @param isRequired Обязательное поле или нет.
 * @param min Минимальное значение.
 * @param max Максимальное значение.
 * @param defaultValue Значение по умолчанию.
 * @param helpText Текст подсказки.
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
 * @param CREATE Создание каталога.
 * @param EDIT Редактирование каталога.
 * @param CREATE_GOODS Создание товара.
 */
export enum ButtonModes {
    CREATE,
    EDIT,
    CREATE_GOODS
}

/**
 * Словарь с режимами.
 * @param CREATE Создание каталога.
 * @param EDIT Редактирование каталога.
 * @param CREATE_GOODS Создание товара.
 */
const modes = new Map([
    [ButtonModes.CREATE, "Создать"],
    [ButtonModes.EDIT, "Изменить"],
    [ButtonModes.CREATE_GOODS, "Добавить"],
]);

/**
 * Свойства компонента CreateBrochureButtonComponent.
 * @param mode Режим работы кнопок компонента.
 * @param goodsStore Хранилище каталогов.
 */
interface CreateBrochureButtonComponentProps extends BaseStoreInjector {
    mode: ButtonModes,
    goodsStore?: GoodsStore,
}

/**
 * Компонент кнопки Создать/Изменить, открывающий свою модалку.
 */
const CreateBrochureButtonComponent: React.FC<CreateBrochureButtonComponentProps> = inject("brochureStore", "goodsStore")(observer((props) => {
    /**
     * Метаданные модалки редактирования каталога.
     */
    const editFormMetadata: Readonly<MetadataProps[]> = [
        { id: "brochure_name", name: "Название", type: MetadataTypes.STR_FIELD, isRequired: true, min: 1, max: 30, helpText: "Значение по длине не более 30 символлов"},
        { id: "brochure_date", name: "Период выпуска каталога", type: MetadataTypes.DATE_FIELD, isRequired: true, defaultValue: new Date().toDateString()},
        { id: "brochure_edition", name: "Тираж", type: MetadataTypes.NMBR_FIELD, isRequired: true, min: 1, max: 10_000, defaultValue: "1", helpText: "Значение не более 10 000"},
    ];

    /**
     * Метаданные для формы создания товаров.
     */
    const createGoodsFormMetaData: Readonly<MetadataProps[]> = [
        { id: "brochure_positions", name: "Перечень товаров", type: MetadataTypes.TBL_FIELD, isRequired: false},
    ];

    /**
     * Метаданные создания каталога.
     */
    const createFormMetadata: Readonly<MetadataProps[]> = [
        ...editFormMetadata,
        ...createGoodsFormMetaData,
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
     * Возвращает метаданные для формы.
     */
    const getFormMetaData = (): readonly MetadataProps[] => {
        switch (props.mode) {
            case ButtonModes.CREATE: return createFormMetadata;
            case ButtonModes.EDIT: return editFormMetadata;
            case ButtonModes.CREATE_GOODS: return createGoodsFormMetaData;
            default: return [];
        }
    };

    /**
     * Возвращает идентификатор формы.
     */
    const getFormId = (): string => {
        let operation;
        switch (props.mode) {
            case ButtonModes.CREATE: operation = "create"; break;
            case ButtonModes.EDIT: operation = "edit"; break;
            case ButtonModes.CREATE_GOODS: operation = "goods_create"; break;
            default: operation = "";
        }
        return `${operation}_form`;
    };

    /**
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
        const metadata = getFormMetaData();//props.mode === ButtonModes.EDIT ? editFormMetadata : createFormMetadata;
        const formId = getFormId();
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
                            help={formItem.helpText}
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

        let response;
        switch (props.mode) {
            case ButtonModes.CREATE: {
                response = props.brochureStore?.handleCreateBrochure(values);
            } break;
            case ButtonModes.EDIT: {
                const brochureEditProps: EditBrochureHandlerProps = {
                    name: values.name,
                    date: values.date,
                    edition: values.edition
                };
                response = props.brochureStore?.handleEditBrochure(brochureEditProps);
            } break;
            case ButtonModes.CREATE_GOODS: {
                response = props.brochureStore?.handleUpdateBrochureGoods();
                props.goodsStore?.updateCurrentBrochureGoods(currentBrochure?.id ?? -1);
            } break;
        }

        response?.then(
            (resolve: string) => {openNotification("Успех", resolve, "success")},
            (error: string) => {openNotification("Ошибка", error, "error")}
        ).finally(() => form.resetFields());
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
                                    (error) => {
                                        console.log("Validate Failed:", error);
                                        return Promise.reject();
                                    }
                                );
    };

    /**
     * Обрабатывает нажатие кнопки отменить.
     */
    const onCancelClick = (): Promise<void> => {
        form.resetFields();
        return Promise.resolve();
    };

    /**
     * Возвращает заголовок модального окна.
     */
    const getModalTitle = (): string => {
        switch (props.mode) {
            case ButtonModes.CREATE: return "Создать каталог";
            case ButtonModes.EDIT: return "Редактировать каталог";
            case ButtonModes.CREATE_GOODS: return "Добавить товар";
            default: return "";
        }
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: getModalTitle(),
        form: form,
        okText: "Сохранить",
        cancelText: "Отменить",
        onOkClick: onOkClick,
        onCancelClick: onCancelClick,
        children: getForm(),
    };

    /**
     * Заполняет поля каталога.
     */
    const fillBrochureFields = () => {
        if (currentBrochure === null) return;

        form.setFieldsValue({
            id: currentBrochure.id,
            name: currentBrochure.name,
            date: moment(currentBrochure.date),
            edition: currentBrochure?.edition,
        });
    };

    /**
     * Заполняет поля товаров.
     */
    const fillGoodsFields = async() => {
        if (currentBrochure === null) return;
        const id = currentBrochure.id ?? -1;
        await props.brochureStore?.updateGoodsList(id);
    };

    /**
     * Заполняет поля данными текалога.
     */
    const fillFieldsOnEdit = (): void => {
        switch (props.mode) {
            case ButtonModes.EDIT: fillBrochureFields(); return;
            case ButtonModes.CREATE_GOODS: fillGoodsFields(); return;
            default: return;
        }
    };

    /**
     * Срабатывает при нажатии кнопки.
     * Используется в качестве callback функции в родительском компоненте.
     */
    const onClick = () => {
        props.mode === ButtonModes.CREATE && props.brochureStore?.updateGoodsList();
        (props.mode === ButtonModes.EDIT || props.mode === ButtonModes.CREATE_GOODS) && fillFieldsOnEdit();
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