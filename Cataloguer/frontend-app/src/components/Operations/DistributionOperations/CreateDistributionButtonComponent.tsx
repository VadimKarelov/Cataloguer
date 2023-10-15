import BaseButton from "../BaseButtonComponent";
import React from "react";
import {MetadataProps, MetadataTypes} from "../BrochureOperations/CreateBrochureButtonComponent";
import {Form, Input, Select} from "antd";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";

/**
 * Свойства компонента содания рассылки.
 */
interface CreateDistributionButtonComponentProps extends BaseStoreInjector {
}

/**
 * Компонент кнопки создания рассылки.
 */
const CreateDistributionButtonComponent: React.FC<CreateDistributionButtonComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Коллекция метаданных.
     */
    const metadata: Readonly<MetadataProps[]> = [
        { id: "distribution_gender", name: "Пол", type: MetadataTypes.LIST_FIELD},
        { id: "distribution_town", name: "Населённый пункт", type: MetadataTypes.LIST_FIELD},
        { id: "distribution_age_group", name: "Возрастная группа", type: MetadataTypes.LIST_FIELD},
        { id: "distribution_brochure_count", name: "Количество каталогов", type: MetadataTypes.NMBR_FIELD},
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
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
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
     * Обрабатывает нажатие кнопки.
     */
    const onButtonClick = () => {
        props.brochureStore?.updateDistributionLists();
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: "Создать рассылку",
        okText: "Сохранить",
        cancelText: "Отменить",
        children: getForm(),
    };

    /**
     * Свойства кнопки.
     */
    const buttonProps = {
        buttonText: "Добавить",
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