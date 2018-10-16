#ifndef BT_ROOT_H
#define BT_ROOT_H

#include "Branch.h"

namespace fluentBehaviorTree
{
	class Root : public Branch
	{
	public:
		Node* mChild;
		void addChild(Node* n);
		Node* copy() override;

		Root();
		~Root() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif