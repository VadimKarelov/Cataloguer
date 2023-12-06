import BaseButton from "../BaseButtonComponent";
import React from "react";
import {MetadataProps, MetadataTypes} from "../BrochureOperations/CreateBrochureButtonComponent";
import {Form, Input, Select} from "antd";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import {DistributionStore} from "../../../stores/DistributionStore";
import {cerr, getValidator} from "../../../Utils";
import {EditDistributionDbProps} from "../../../types/DistributionTypes";
import {openNotification} from "../../NotificationComponent";

/**
 * Свойства компонента содания рассылки.
 * @param row Строка таблицы.
 * @param distributionStore Хранилище рассылок.
 */
interface CreateDistributionButtonComponentProps extends BaseStoreInjector {
    row?: EditDistributionDbProps,
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
                            initialValue={formItem.defaultValue}
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

                                        const {ageGroupId, genderId, townId, brochureCount} = props.row;

                                        form.setFieldsValue({
                                            ageGroups: `ageGroups_${ageGroupId}`,
                                            genders: `genders_${genderId}`,
                                            towns: `towns_${townId}`,
                                            brochureCount: brochureCount,
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

        const baseObject = {
            brochureId: brochureId,
            ageGroupId: ageGroupId,
            genderId: genderId,
            townId: townId,
            brochureCount: parseFloat(brochureCount)
        };

        const objectToEdit = {
            id: props.row?.id ?? -1,
            ...baseObject
        };

        const response = !props.row ?
            props.distributionStore?.handleCreateBrochureDistribution(baseObject)
            : props.distributionStore?.handleEditBrochureDistribution(objectToEdit);

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
                    cerr(`Validate Failed: ${error}`);
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
     * Возвращает настройки хинта.
     */
    const getTooltipProps = () => {
        const title = "";
        return {title: title};
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
        onCancelClick: onCancelClick,
    };

    /**
     * Свойства кнопки.
     */
    const buttonProps = {
        buttonText: `${!props.row ? "Добавить" : "Изменить"}`,
        onClick: onButtonClick,
        tooltip: getTooltipProps(),
    };

    return (
        <BaseButton
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CreateDistributionButtonComponent;