import { Component } from '@angular/core';
import { NavController, ActionSheetController } from 'ionic-angular';

@Component({
  selector: 'page-content',
  templateUrl: 'content.html'
})
export class ContentPage {

  constructor(public navCtrl: NavController, public actionSheetCtrl: ActionSheetController) {
    console.log(1);
    this.items = [
      '11111111111111111111111111111111111111111111111111111111111111111111',
      '11111111111111111111111111111111111111111111111111111111111111111111',
      '11111111111111111111111111111111111111111111111111111111111111111111'];
  }
  items: string[];

  buttons: any = [
    {
      text: 'Destructive1',
      role: 'destructive',
      handler: () => {
        console.log('Destructive clicked');
      }
    }, {
      text: 'Archive2',
      handler: () => {
        console.log('Archive clicked');
      }
    }, {
      text: 'Cancel',
      role: 'cancel',
      handler: () => {
        console.log('Cancel clicked');
      }
    }
  ]
  presentActionSheet = () => {
    const actionSheet = this.actionSheetCtrl.create({
      title: 'Modify your album',
      buttons: this.buttons
    });
    console.log(1);
    actionSheet.present();
  }
}
