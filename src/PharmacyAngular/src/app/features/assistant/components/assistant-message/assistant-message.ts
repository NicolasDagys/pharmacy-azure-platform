import {
  ChangeDetectionStrategy,
  Component,
  Input
} from '@angular/core';

import { DatePipe } from '@angular/common';

import { AssistantMessage } from '../../models/assistant-message';

@Component({
  selector: 'app-assistant-message',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './assistant-message.html',
  styleUrl: './assistant-message.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssistantMessageComponent {

  @Input({ required: true })
  message!: AssistantMessage;

}