import { IPartner } from "./IPartner";

export class Partner implements IPartner {
    constructor(
        public username: string,
        public payPalId: string,
        public id: string,
        public site: string,
        public userPattern: string,
        public creatorWebhook: string
    ) { }

}