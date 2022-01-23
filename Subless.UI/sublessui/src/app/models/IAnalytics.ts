import { IUserStats } from "./IUserStats";

export interface IAnalytics {
    lastMonth: IUserStats;
    thisMonth: IUserStats;
}
