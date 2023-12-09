import {action, makeAutoObservable, observable} from "mobx";
import {
    BrochureProps,
    CreateBrochureHandlerProps,
    DistributionProps,
    EditBrochureHandlerProps,
    GoodDBProps,
    GoodProps,
    GoodsExtendedProps,
    GoodsProps, RunPointsDbProps,
    StatusArrayProps
} from "../types/BrochureTypes";
import {cerr, cout, random} from "../Utils";
import {ageGroups, genders, goodsNames, towns} from "./HandbookExamples";
import GoodsService from "../services/GoodsService";
import BrochureService from "../services/BrochureService";
import dayjs from "dayjs";
import {SHOULD_USE_ONLY_DB_DATA} from "../constants/EnvironmentVariables";

/**
 * Путь к данным каталога в session storage.
 */
const BROCHURE_SS_PATH: Readonly<string> = "ss_brochure";

/**
 * Перечислимый тип для статусов.
 */
export enum Status {
    DRAFT,
    RELEASED,
}

/**
 * Набор возможных статусов.
 */
export const BROCHURE_STATUSES: Readonly<StatusArrayProps[]> = [
    {key: Status.DRAFT, value: "Не выпущен"},
    {key: Status.RELEASED, value: "Выпущен"},
];

/**
 * Хранилище данных для каталога.
 */
class BrochureStore {
    /**
     * Выбран ли каталог.
     */
    @observable public isBrochureSelected: boolean;

    /**
     * Загружается ли каталог.
     */
    @observable public isBrochureLoading: boolean;

    /**
     * Загружается ли меню каталогов.
     */
    @observable public isBrochureMenuLoading: boolean;

    /**
     * Коллекция каталогов.
     */
    @observable public brochures: BrochureProps[];

    /**
     * Текущий (выбранный) каталог.
     */
    @observable public currentBrochure: BrochureProps | null;

    /**
     * Все возможные товары.
     * Необходимы в форме создания каталога.
     */
    @observable public allGoods: GoodsProps[];

    /**
     * Отмеченные товары.
     */
    @observable private checkedGoods: GoodDBProps[];

    /**
     * Загружаются ли товары.
     */
    @observable public isLoadingGoods: boolean;

    /**
     * Множество точек для постороения графика.
     */
    @observable public runPoints: RunPointsDbProps[];

    /**
     * Множество точек для постороения графика предсказанных значений.
     */
    @observable public predictedRunPoints: RunPointsDbProps[];

    /**
     * Загружается ли график.
     */
    @observable public isLoadingChart: boolean;

    /**
     * Конструктор.
     */
    constructor() {
        this.brochures = [];
        this.allGoods = [];
        this.checkedGoods = [];
        this.runPoints = [];
        this.predictedRunPoints = [];
        this.currentBrochure = null;
        this.isBrochureSelected = false;
        this.isBrochureLoading = false;
        this.isBrochureMenuLoading = false;
        this.isLoadingGoods = false;
        this.isLoadingChart = false;

        makeAutoObservable(this);

        this.loadBrochures = this.loadBrochures.bind(this);
        this.onBrochureClick = this.onBrochureClick.bind(this);
        this.initBrochures = this.initBrochures.bind(this);
        this.getRandomGoods = this.getRandomGoods.bind(this);
        this.getRandomDistributions = this.getRandomDistributions.bind(this);
        this.getSavedBrochure = this.getSavedBrochure.bind(this);
        this.saveBrochureToSessionStorage = this.saveBrochureToSessionStorage.bind(this);
        this.getSavedBrochureMenu = this.getSavedBrochureMenu.bind(this);
        this.reset = this.reset.bind(this);

        this.getAllGoods = this.getAllGoods.bind(this);
        this.getUnselectedGoods = this.getUnselectedGoods.bind(this);
        this.setCheckedGoods = this.setCheckedGoods.bind(this);
        this.updateGoodsList = this.updateGoodsList.bind(this);
        this.handleUpdateBrochureGoods = this.handleUpdateBrochureGoods.bind(this);

        this.updateBrochureData = this.updateBrochureData.bind(this);
        this.handleEditBrochure = this.handleEditBrochure.bind(this);
        this.handleCreateBrochure = this.handleCreateBrochure.bind(this);
        this.getBrochureById = this.getBrochureById.bind(this);
        this.updateBrochureList = this.updateBrochureList.bind(this);
        this.handleDeleteBrochure = this.handleDeleteBrochure.bind(this);
        this.getRunData = this.getRunData.bind(this);
        this.updateRunChartPoints = this.updateRunChartPoints.bind(this);
        this.startBrochureRun = this.startBrochureRun.bind(this);
        this.releaseBrochure = this.releaseBrochure.bind(this);
    }

    /**
     * Выпускает каталог.
     */
    @action public async releaseBrochure() {
        const brochureId = this.currentBrochure?.id ?? -1;
        if (brochureId === -1) {
            return Promise.reject(`Каталог не получилось выпустить`);
        }

        return await BrochureService.releaseBrochure(brochureId).then(
            (response) => {
                const data = response.data;
                if (data !== "OK") {
                    return Promise.reject(`Каталог не получилось выпустить`);
                }

                this.onBrochureClick(brochureId);
                this.loadBrochures();
                return Promise.resolve(`Каталог успешно выпущен`);
            },
            (error) => {
                cerr(error);
                return Promise.reject(`Каталог не получилось выпустить`);
            },
        );
    }

    /**
     * Запускает расчёт.
     */
    @action public async startBrochureRun() {
        const brochureId = this.currentBrochure?.id ?? -1;
        if (brochureId === -1) {
            return Promise.reject(`Расчёт завершился с ошибкой`);
        }

        return await BrochureService.startBrochureRun(brochureId).then(
            (response) => {
                const data = response.data;
                cout(data);

                if (isNaN(parseFloat(data))) {
                    return Promise.reject(data);
                }

                this.onBrochureClick(brochureId);
                this.loadBrochures();
                this.updateRunChartPoints();
                return Promise.resolve(`Расчёт выполнился успешно. Эффективность составляет ${data}`);
            },
            (error) => {
                cerr(error);
                return Promise.reject(`Расчёт завершился с ошибкой`);
            },
        );
    }

    /**
     * Обновляет коллекцию точек для графика.
     */
    @action public updateRunChartPoints() {
        this.isLoadingChart = true;
        const brochureId: number = this.currentBrochure?.id ?? -1;
        if (brochureId === -1) return;
        Promise.all([
            BrochureService.getRunData(brochureId),
            BrochureService.getPredictedRunData(brochureId),
        ]).then(
            (responses) => {
                const runData = responses[0].data;
                const predictedRunData = responses[1].data;

                if (runData instanceof Array) {
                    this.runPoints = runData;
                }

                if (predictedRunData instanceof Array) {
                    this.predictedRunPoints = predictedRunData;
                }
            },
            (error) => {
                cerr(error);
            },
        ).finally(() => this.isLoadingChart = false);
    }

    /**
     * Возвращает множество точек для графика расчёта.
     * @param brochureId Идентификатор каталога.
     * @private
     */
    @action private async getRunData(brochureId: number) {
        return await BrochureService.getRunData(brochureId);
    }

    /**
     * Обработчик удаления каталога
     * @param brochureId Идентификатор каталога.
     */
    @action public async handleDeleteBrochure(brochureId: number): Promise<string> {
        if (brochureId === -1) {
            return Promise.reject("Ошибка при удалении каталога");
        }

        return await BrochureService.deleteBrochure(brochureId).then(
            async(response) => {
                await this.updateBrochureList();

                this.currentBrochure = null;
                this.saveBrochureToSessionStorage(null);

                const data = response.data;
                if (!isNaN(parseInt(data)) && parseInt(data) === -1) {
                    return Promise.reject("Ошибка при удалении каталога");
                }
                return Promise.resolve("Каталог удалён успешно");
            },
            (error) => {
                cerr(error);
                return Promise.reject("Ошибка при удалении каталога");
            }
        );
    }

    /**
     * Обрабатывает создание каталога.
     * @param brochure Каталог.
     */
    @action public async handleEditBrochure(brochure: EditBrochureHandlerProps): Promise<string> {
        const id = this.currentBrochure?.id ?? -1;
        const rejectReason = "Не удалось изменить каталог";
        if (id === -1) {
            return Promise.reject(rejectReason);
        }

        return await BrochureService.updateBrochure(id, brochure).then(
            async(response) => {
                const data = response.data;

                const isResponseString = typeof data === "string";
                const isResponseNumber = typeof data === "number";
                const numberParseAttempt = parseInt(data);

                if (!isResponseString && !isResponseNumber) {
                    return Promise.reject(rejectReason);
                }

                if (isResponseString && !isNaN(numberParseAttempt) && numberParseAttempt === -1) {
                    return Promise.reject(rejectReason);
                }

                if (isResponseNumber && data === -1) {
                    return Promise.reject(rejectReason);
                }
                await this.updateBrochureData(id);

                this.isBrochureLoading = false;
                this.isBrochureMenuLoading = false;
                return Promise.resolve("Каталог успешно изменён");
            },
            (error) => {
                cerr(error);
                this.isBrochureLoading = false;
                this.isBrochureMenuLoading = false;
                return Promise.reject(rejectReason);
            },
        );
    }

    /**
     * Вызывает запрос на получения данных о каталоге по идентификатору.
     * @param id Идентификатор каталога.
     */
    @action public async getBrochureById(id: number) {
        return await BrochureService.getBrochureById(id);
    }

    /**
     * Обновляет данные выбранного каталога.
     * @param id Идентификатор каталога.
     * @private
     */
    @action private async updateBrochureData(id: number) {
        const responses =  await Promise.all([
            BrochureService.getAllBrochures(),
            BrochureService.getBrochureById(id),
        ]);

        const brochuresAxiosResponse = responses[0];
        const brochureAxiosResponse = responses[1];

        if (brochuresAxiosResponse && brochuresAxiosResponse.data instanceof Array) {
            this.brochures = brochuresAxiosResponse.data;
        }

        if (brochureAxiosResponse) {
            await this.onBrochureClick(id);
            // this.currentBrochure = brochureAxiosResponse.data;
        }
    }

    /**
     * Обрабатывает создание каталога.
     * @param brochure Каталог.
     */
    @action public async handleCreateBrochure(brochure: CreateBrochureHandlerProps) {
        brochure.positions = [...this.checkedGoods.map(good => ({...good}))];
        this.isBrochureLoading = true;
        this.isBrochureMenuLoading = true;
        const {data} = await BrochureService.createBrochure(brochure);

        const id = data;
        if (!(typeof id === "number") || id === -1) {
            this.isBrochureLoading = false;
            this.isBrochureMenuLoading = false;
            return Promise.reject("Ошибка при создании каталога");
        }

        await this.updateBrochureData(id);

        this.isBrochureLoading = false;
        this.isBrochureMenuLoading = false;
        return Promise.resolve("Каталог успешно создан");
    }

    /**
     * Обрабатывает изменение товаров в каталоге.
     * @param updateGoods Функция обновления товаров.
     * Используется в качетсве параметра, так как находится внутри другого хранилища.
     */
    @action public async handleUpdateBrochureGoods(updateGoods: (brochureId: number) => Promise<void>): Promise<string> {
        const brochureId = this.currentBrochure?.id ?? -1;
        if (brochureId === -1) {
            return Promise.reject("Не удалось добавить товары в каталог");
        }

        return await GoodsService.addGoodsToBrochure(brochureId, this.checkedGoods).then(
            (response) => {
                const data = response.data;

                if (!isNaN(parseInt(data)) && parseInt(data) === -1) {
                    return Promise.reject("Не удалось добавить товары в каталог");
                }

                updateGoods(brochureId);
                return Promise.resolve("Товары успешно добавлены в каталог");
            },
            (error) => {
                cerr(error);
                return Promise.reject("Не удалось добавить товары в каталог");
            },
        );
    }

    /**
     * Сохраняет отмеченные строки.
     * @param goods Товары.
     */
    @action public setCheckedGoods(goods: GoodsExtendedProps[]) {
        this.checkedGoods = goods
            .filter(good => good.isChecked)
            .map((good) => ({id: good.id, price: parseFloat(good.price.toString())}));
    }

    /**
     *
     * Получает и записывает все возможные товары каталога.
     * @param brochureId Идентификатор каталога.
     */
    @action public async getUnselectedGoods(brochureId: number) {
        return await GoodsService.getUnselectedBrochureGoods(brochureId).then(
            ((response: {data: any}) => {
                const isFine = response.data instanceof Array;
                if (!isFine) return;

                this.allGoods = response.data;
            }),
            (error) => cerr(error)
        );
    }

    /**
     * Получает и записывает все возможные товары каталога.
     */
    @action public async getAllGoods() {
        return await GoodsService.getGoods().then(
            ((response: {data: any}) => {
                const isFine = response.data instanceof Array;
                if (!isFine) return;

                this.allGoods = response.data;
            }),
            (error) => cerr(error)
        );
    }

    /**
     * Обновляет список товаров для создания каталога.
     */
    @action public async updateGoodsList(brochureId = -1) {
        this.allGoods = [];
        this.checkedGoods = [];

        this.isLoadingGoods = true;
        await (brochureId === -1 ?
            this.getAllGoods()
            : this.getUnselectedGoods(brochureId)
        ).finally(() => this.isLoadingGoods = false);
    }

    /**
     * Очищает данные о каталоге.
     */
    @action public reset(): void {
        this.currentBrochure = null;
        sessionStorage.removeItem(BROCHURE_SS_PATH);
    }

    /**
     * Возвращает случайные товары.
     */
    private getRandomGoods(): GoodProps[] {
        const goodCount = 10;
        const goods: GoodProps[] = [];

        for (let i = 0; i < goodCount; i++) {
            const tempName = `${goodsNames.at(random(0, goodsNames.length - 1))}_${i}`;
            goods.push(
                {name: tempName, price: random(0, 10000)}
            );
        }

        return goods;
    }

    /**
     * Возвращает случайные рассылки.
     */
    private getRandomDistributions(): DistributionProps[] {
        const brochureCount = 20;
        const distributions: DistributionProps[] = [];

        for (let i = 0; i < brochureCount; i++) {
            const town = towns[random(0, towns.length - 1)];
            const ageGroup = ageGroups[random(0, ageGroups.length - 1)];
            const gender = genders[random(0, genders.length - 1)];
            distributions.push(
                {count: i + brochureCount, town: town, gender: gender, ageGroup: ageGroup}
            );
        }

        return distributions;
    }

    /**
     * Инициализирует коллекцию каталогов.
     */
    private initBrochures(): void {
        let tempBrochures: BrochureProps[] = [];
        const brochureCount = 100;
        for (let i = 0; i < brochureCount; ++i) {
            const tempCount = random(1, 5000);
            const tempCreationDate = dayjs(new Date()).toDate().toLocaleString();
            const tempGoods = this.getRandomGoods();
            const tempDistributions = this.getRandomDistributions();
            const status = BROCHURE_STATUSES[random(0, BROCHURE_STATUSES.length - 1)].value;
            tempBrochures.push({
                id: i,
                name: `Каталог ${i}`,
                edition: tempCount,
                date: tempCreationDate,
                goods: tempGoods,
                distributions: tempDistributions,
                statusName: status,
                potentialIncome: 0
            });
        }
        this.brochures = tempBrochures;
    }

    /**
     * Возвращает меню с открытыми каталогами из session storage.
     */
    @action public getSavedBrochureMenu(): string[] {
        const brochure = this.getSavedBrochure();
        return brochure ? [`brochure_${brochure.id}`] : [];
    }

    /**
     * Возвращает сохранённый каталог в сессии браузера.
     */
    @action public getSavedBrochure(): BrochureProps | null {
        const json = sessionStorage.getItem(BROCHURE_SS_PATH);
        if (json === null || json === "undefined") return null;
        return JSON.parse(json);
    }

    /**
     * Сохраняет каталог  в session storage.
     * @param brochure - каталог.
     */
    @action private saveBrochureToSessionStorage(brochure: BrochureProps | null): void {
        sessionStorage.setItem(BROCHURE_SS_PATH, JSON.stringify(brochure));
    }

    /**
     * Срабатывает после выбора каталога в меню.
     * @param brochureId - идентификатор каталога.
     */
    @action public async onBrochureClick(brochureId: number) {
        if (isNaN(brochureId) || brochureId === -1) return;

        this.isBrochureSelected = true;
        this.isBrochureLoading = true;

        let foundBrochure = null;

        // if (brochureId !== -1) {
            if (SHOULD_USE_ONLY_DB_DATA === "false") {
                foundBrochure = this.brochures.find(brochure => brochure.id === brochureId) ?? null;
            } else {
                await this.getBrochureById(brochureId).then((response: {data: any}) => {
                    foundBrochure = response.data;
                });
            }
        // }

        this.currentBrochure = foundBrochure;
        this.saveBrochureToSessionStorage(foundBrochure);

        setTimeout(() => {
            this.isBrochureLoading = false;
        }, 250);
    }

    /**
     * Обновляет список каталогов.
     * @private
     */
    @action private async updateBrochureList() {
        await BrochureService.getAllBrochures().then(
            (response) => {
                const data = response.data;
                if (!(data instanceof Array)) return;

                this.brochures = data;
            },
            (error) => cerr(error)
        );
    }

    /**
     * Загружает каталоги.
     */
    @action public loadBrochures() {
        this.isBrochureMenuLoading = true;

        SHOULD_USE_ONLY_DB_DATA === "false" ? this.initBrochures() : this.updateBrochureList();

        setTimeout(() => {
            this.isBrochureMenuLoading = false;
        }, 300);
    }
}

export default BrochureStore;