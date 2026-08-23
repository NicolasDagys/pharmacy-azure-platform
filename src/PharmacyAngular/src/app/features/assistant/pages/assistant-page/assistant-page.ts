import {
  ChangeDetectionStrategy,
  Component
} from '@angular/core';

import { AssistantChatComponent } from '../../components/assistant-chat/assistant-chat';

@Component({
  selector: 'app-assistant-page',
  standalone: true,
  imports: [
    AssistantChatComponent
  ],
  templateUrl: './assistant-page.html',
  styleUrl: './assistant-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssistantPageComponent {
}