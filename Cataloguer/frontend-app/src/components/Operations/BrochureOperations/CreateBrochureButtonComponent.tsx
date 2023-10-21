import BaseButton from "../BaseButtonComponent";
import React from "react";
import {Button, DatePicker, Form, Input, Popconfirm, Space, Table} from "antd";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import {inject, observer} from "mobx-react";
import CreateDistributionButtonComponent from "../DistributionOperations/CreateDistributionButtonComponent";

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

const columns = [
    {
        title: "Название",
        dataIndex: "name",
        key: "goods_table_name",
        // width: 315
    },
    {
        title: "Цена",
        dataIndex: "cost",
        key: "goods_table_cost",
        // width: 128
    },
];

// rowSelection object indicates the need for row selection
const rowSelection = {
    onChange: (selectedRowKeys: React.Key[], selectedRows: any[]) => {
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: (record: any) => ({
        disabled: record.name === 'Disabled User', // Column configuration not to be checked
        name: record.name,
    }),
};

/**
 * Свойства компонента CreateBrochureButtonComponent.
 * @param mode Режим работы кнопок компонента.
 */
interface CreateBrochureButtonComponentProps extends BaseStoreInjector {
    mode: ButtonModes
}

/**
 * Компонент кнопки Создать/Изменить, открывающий свою модалку.
 */
const CreateBrochureButtonComponent: React.FC<CreateBrochureButtonComponentProps> = inject("brochureStore")(observer((props) => {
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
     * Все возможные товары.
     */
    const allGoods = props.brochureStore?.allGoods ?? [];


    /**
     * Возвращает компонет элемента формы.
     * @param formItem Элемент формы.
     */
    const getFormItemComponent = (formItem: MetadataProps) => {
        switch (formItem.type) {
            case MetadataTypes.NMBR_FIELD:
            case MetadataTypes.STR_FIELD: return (<Input/>);
            case MetadataTypes.DATE_FIELD: return (<DatePicker/>);
            case MetadataTypes.TBL_FIELD: return (<Table columns={columns} rowSelection={{
                type: 'checkbox',
                ...rowSelection,
            }}/>);
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

    const onClick = () => {
        props.brochureStore?.updateGoodsList();
    };

    const buttonProps = {
        buttonText: currentMode,
        onClick: onClick
    };

    return (
        <BaseButton
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CreateBrochureButtonComponent;