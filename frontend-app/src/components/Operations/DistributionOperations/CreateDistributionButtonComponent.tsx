import BaseButton from "../BaseButtonComponent";
import React from "react";
import {MetadataProps, MetadataTypes} from "../BrochureOperations/CreateBrochureButtonComponent";
import {Form, Input, Select} from "antd";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector, customProp, DistributionProps} from "../../../types/BrochureTypes";
import {CreateDistributionDbProps, DistributionStore} from "../../../stores/DistributionStore";
import {getValidator} from "../../../Utils";

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
        { id: "distribution_genders", name: "Пол", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_towns", name: "Населённый пункт", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_ageGroups", name: "Возрастная группа", type: MetadataTypes.LIST_FIELD, isRequired: true},
        { id: "distribution_brochureCount", name: "Количество каталогов", type: MetadataTypes.NMBR_FIELD, isRequired: true, min: 1, defaultValue: '1'},
    ];

    /**
     * Указатель на форму.
     */
    const [form] = Form.useForm();

    /**
     * Возвращает набор элементов для списка select.
     * @param formItem
     */
    const getSelectOptions = (formItem: MetadataProps) => {
        const formId = formItem.id;
        const formName = formId.slice(formId.lastIndexOf('_') + 1);
        let arr: any[] = [];

        switch (formName) {
            case "genders": arr = props.distributionStore?.getGenders() ?? []; break;
            case "ageGroups": arr = props.distributionStore?.getAgeGroups() ?? []; break;
            case "towns": arr = props.distributionStore?.getTowns() ?? []; break;
        }

        return arr.map(item => (<Select.Option key={`${formName}_${item.id}`}>{item.name}</Select.Option>));
    };

    /**
     * Возвращает компонет элемента формы.
     * @param formItem Элемент формы.
     */
    const getFormItemComponent = (formItem: MetadataProps) => {
        switch (formItem.type) {
            case MetadataTypes.NMBR_FIELD: return (<Input type={"number"}/>);
            case MetadataTypes.STR_FIELD: return (<Input/>);
            case MetadataTypes.LIST_FIELD: return (<Select placeholder={`Выбрать '${formItem.name.toLowerCase()}'`}>{getSelectOptions(formItem)}</Select>);
            default: return null;
        }
    };

    /**
     * Возвращает значение для инициализации поля формы.
     * @param key Название поля.
     */
    // const getInitValue = (key: string) => {
    //     const isEditButton = !!props.row;
    //     if (!isEditButton) return null;
    //
    //     const finalKey = key.slice(key.lastIndexOf('_') + 1)
    //     const parsable: customProp = props.row ?? {};
    //     return parsable[finalKey] ?? null;
    // };

    /**
     * Возвращает компонент формы.
     */
    const getForm = (): React.JSX.Element => {
        return (
            <Form form={form} id={`distribution_${!props.row ? "create" : "edit"}`} layout={"vertical"} colon={false}>
                {metadata.map(formItem => {
                    const formId = formItem.id;
                    const formName = formId.slice(formId.lastIndexOf('_') + 1);
                    return (
                        <Form.Item
                            key={formItem.id}
                            label={formItem.name}
                            name={formName}
                            initialValue={formItem.defaultValue} /*initialValue={getInitValue(formItem.id)}*/
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
     * Обрабатывает нажатие кнопки.
     */
    const onButtonClick = (): void => {
        props.distributionStore?.updateDistributionLists()
                                    .finally(() => {
                                        if (!props.row) return;

                                        const {ageGroup, gender, town, count} = props.row;

                                        form.setFieldsValue({
                                            ageGroups: ageGroup,
                                            genders: gender,
                                            towns: town,
                                            brochureCount: count,
                                        });
                                    });
    };

    /**
     * Возвращают идентификатор поля.
     * @param field Поле (ключ) с идентификатором.
     */
    const getId = (field: string): number => {
        const parsedIdValue = field.slice(field.lastIndexOf('_') + 1);
        return parseFloat(parsedIdValue);
    };

    /**
     * Обработчик создания/редактирования расслки.
     * @param values Значения полей.
     */
    const handleDbAction = (values: any) => {
        const {ageGroups, genders, towns, brochureCount} = values;
        const ageGroupId = getId(ageGroups);
        const genderId = getId(genders);
        const townId = getId(towns);

        if (isNaN(ageGroupId) || isNaN(genderId) || isNaN(townId)) {
            throw new Error("Проблема с парсингом id");
        }

        const brochureId = props.brochureStore?.currentBrochure?.id ?? -1;

        if (brochureId === -1) {
            throw new Error("Приколы с каталогом");
        }

        const finalObject: CreateDistributionDbProps = {
            brochureId: brochureId,
            ageGroupId: ageGroupId,
            genderId: genderId,
            townId: townId,
            brochureCount: parseFloat(brochureCount)
        };

        props.distributionStore?.handleCreateBrochureDistribution(finalObject);
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
                (error) => {
                    console.log("Validate Failed:", error);
                    return Promise.reject();
                }
            );
    };

    /**
     * Свойства модального окна.
     */
    const modalProps = {
        title: `${!props.row ? "Создать" : "Редактировать"} рассылку`,
        okText: `${!props.row ? "Сохранить" : "Редактировать"}`,
        cancelText: "Отменить",
        children: getForm(),
        onOkClick: onOkClick,
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