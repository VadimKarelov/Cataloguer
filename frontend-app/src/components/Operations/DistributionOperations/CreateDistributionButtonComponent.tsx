import BaseButton from "../BaseButtonComponent";
import React from "react";
import {MetadataProps, MetadataTypes} from "../BrochureOperations/CreateBrochureButtonComponent";
import {Form, Input, Select} from "antd";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector, customProp, DistributionProps} from "../../../types/BrochureTypes";
import {DistributionStore} from "../../../stores/DistributionStore";

/**
 * Свойства компонента содания рассылки.
 */
interface CreateDistributionButtonComponentProps extends BaseStoreInjector {
    row?: DistributionProps,
    distributionStore?: DistributionStore,
}

/**
 * Компонент кнопки создания рассылки.
 */
const CreateDistributionButtonComponent: React.FC<CreateDistributionButtonComponentProps> = inject("brochureStore", "distributionStore")(observer((props) => {
    /**
     * Коллекция метаданных.
     */
    const metadata: Readonly<MetadataProps[]> = [
        { id: "distribution_gender", name: "Пол", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_town", name: "Населённый пункт", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_ageGroup", name: "Возрастная группа", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_brochure_count", name: "Количество каталогов", type: MetadataTypes.NMBR_FIELD, isRequired: true},
    ];

    /**
     * Возвращает компонет элемента формы.
     * @param formItem Элемент формы.
     */
    const getFormItemComponent = (formItem: MetadataProps) => {
        switch (formItem.type) {
            case MetadataTypes.NMBR_FIELD:
            case MetadataTypes.STR_FIELD: return (<Input/>);
            case MetadataTypes.LIST_FIELD: return (<Select/>);
            default: return null;
        }
    };

    /**
     * Возвращает значение для инициализации поля формы.
     * @param key Название поля.
     */
    const getInitValue = (key: string) => {
        const isEditButton = !!props.row;
        if (!isEditButton) return null;

        const finalKey = key.slice(key.lastIndexOf('_') + 1)
        const parsable: customProp = props.row ?? {};
        return parsable[finalKey] ?? null;
    };

    /**
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
        return (
            <Form layout={"vertical"} colon={false}>
                {metadata.map(formItem => {
                    const formId = formItem.id;
                    const formName = formId.slice(formId.lastIndexOf('_') + 1);
                    return (
                        <Form.Item key={formItem.id} label={formItem.name} name={formName} initialValue={getInitValue(formItem.id)}>
                            {getFormItemComponent(formItem)}
                        </Form.Item>
                    );
                })}
            </Form>
        );
    };

    /**
     * Обрабатывает нажатие кнопки.
     */
    const onButtonClick = () => {
        props.distributionStore?.updateDistributionLists();
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: `${!props.row ? "Создать" : "Редактировать"} рассылку`,
        okText: `${!props.row ? "Сохранить" : "Редактировать"}`,
        cancelText: "Отменить",
        children: getForm(),
    };

    /**
     * Свойства кнопки.
     */
    const buttonProps = {
        buttonText: `${!props.row ? "Добавить" : "Изменить"}`,
        onClick: onButtonClick,
    };

    return (
        <BaseButton
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CreateDistributionButtonComponent;